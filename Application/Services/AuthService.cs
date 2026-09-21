using PayRollApi.Application.Common;
using PayRollApi.Application.DTOs;
using PayRollApi.Application.Interfaces;
using PayRollApi.Application.Interfaces.UOWInterfaces;
using PayRollApi.Application.Logging;
using PayRollApi.Domain.Interfaces;
using FluentValidation;
using System;

namespace PayRollApi.Application.Services
{
    internal class AuthService(
        ITokenService tokenService,
        IPasswordHashing passwordHashing,
        IGetUser getUser,
        ITokenUOW tokenUOW,
        ILocalizer localizer,
        IApplicationLogger logger,
        IValidator<LoginRequest> validator) : IAuthService
    {
        public async Task<ApiResponse<AuthResult>> Login(LoginRequest request)
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
                return new FailResponse<AuthResult>(localizer.Get("InvalidCredentials"))
                {
                    Errors = validation.Errors.GroupBy(x => x.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(x => localizer.Get(x.ErrorMessage)).ToArray())
                };

            var user = await getUser.GetByUsername(request.Username);

            // Same message for unknown user and wrong password — don't leak which one it was.
            if (user is null || !user.IsActive ||
                !passwordHashing.VerifyBcryptHashPassword(request.Password, user.PasswordHash))
            {
                logger.LogWarning("Login attempt failed for username {Username}", request.Username);
                return new FailResponse<AuthResult>(localizer.Get("InvalidCredentials"), System.Net.HttpStatusCode.Unauthorized);
            }

            // Quietly upgrade old hashes whenever the work factor goes up.
            if (passwordHashing.PasswordNeedsRehash(user.PasswordHash))
            {
                user.PasswordHash = passwordHashing.HashPassword(request.Password);
                await tokenUOW.UsersCRUD.Update(user);
            }

            user.LastLoginAt = DateTime.UtcNow;
            await tokenUOW.UsersCRUD.Update(user);

            var accessToken = tokenService.GenerateAccessToken(user);
            var (rawRefreshToken, refreshTokenEntity) = tokenService.GenerateRefreshToken(user.Id);
            await tokenUOW.RefreshTokenCRUD.Create(refreshTokenEntity);
            await tokenUOW.CompleteAsync();

            logger.LogInformation("User {Username} logged in successfully", request.Username);

            return new ApiResponse<AuthResult>
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Message = localizer.Get("LoginSuccess"),
                Object = new AuthResult(
                    accessToken,
                    DateTime.UtcNow.AddMinutes(AccessTokenMinutes),
                    rawRefreshToken,
                    refreshTokenEntity.ExpiresAt,
                    user.Id,
                    user.Username)
            };
        }

        public async Task<ApiResponse<AuthResult>> Refresh(string? rawRefreshToken)
        {
            if (string.IsNullOrWhiteSpace(rawRefreshToken))
                return SessionExpired();

            var tokenHash = tokenService.HashRefreshToken(rawRefreshToken);
            var storedToken = await tokenUOW.RefreshTokenCRUD.GetByTokenHash(tokenHash);

            if (storedToken is null)
                return SessionExpired();

            if (storedToken.RevokedAt is not null)
            {
                // The same token was presented twice — treat it as stolen and kill the whole chain.
                logger.LogWarning("Reused refresh token detected for admin user {AdminUserId}", storedToken.AdminUserId);
                await tokenUOW.RefreshTokenCRUD.RevokeAllActiveForUser(storedToken.AdminUserId);
                await tokenUOW.CompleteAsync();
                return SessionExpired();
            }

            if (storedToken.IsExpired)
                return SessionExpired();

            var user = await tokenUOW.UsersCRUD.GetById(storedToken.AdminUserId);
            if (user is null || !user.IsActive)
                return SessionExpired();

            var newAccessToken = tokenService.GenerateAccessToken(user);
            var (newRawRefreshToken, newRefreshTokenEntity) = tokenService.GenerateRefreshToken(user.Id);

            // Rotate: the old token dies, the new one takes its place.
            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.ReplacedByTokenHash = newRefreshTokenEntity.TokenHash;
            await tokenUOW.RefreshTokenCRUD.Create(newRefreshTokenEntity);
            await tokenUOW.CompleteAsync();

            return new SuccessResponse<AuthResult>(localizer.Get("LoginSuccess"), new AuthResult(
                newAccessToken,
                DateTime.UtcNow.AddMinutes(AccessTokenMinutes),
                newRawRefreshToken,
                newRefreshTokenEntity.ExpiresAt,
                user.Id,
                user.Username));
        }

        // Same vague answer for every failure so nobody can probe which tokens exist.
        private FailResponse<AuthResult> SessionExpired() =>
            new(localizer.Get("SessionExpired"), System.Net.HttpStatusCode.Unauthorized);

        public async Task Logout(string? rawRefreshToken)
        {
            if (string.IsNullOrWhiteSpace(rawRefreshToken))
                return;

            var tokenHash = tokenService.HashRefreshToken(rawRefreshToken);
            var storedToken = await tokenUOW.RefreshTokenCRUD.GetByTokenHash(tokenHash);
            if (storedToken is null || storedToken.RevokedAt is not null)
                return;

            storedToken.RevokedAt = DateTime.UtcNow;
            await tokenUOW.CompleteAsync();
        }

        public async Task<ApiResponse<CurrentUserDto>> Me(int adminUserId)
        {
            var user = await tokenUOW.UsersCRUD.GetById(adminUserId);
            if (user is null)
                return new FailResponse<CurrentUserDto>(localizer.Get("NotFound"), System.Net.HttpStatusCode.NotFound);

            return new SuccessResponse<CurrentUserDto>(localizer.Get("SuccessRetrieving"), new CurrentUserDto(user.Id, user.Username));
        }

        private const int AccessTokenMinutes = 15;
    }
}
