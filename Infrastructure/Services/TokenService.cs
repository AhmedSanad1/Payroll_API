using PayRollApi.Domain.Entities.SecurityModule;
using PayRollApi.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PayRollApi.Infrastructure.Services
{
    public class TokenService(IConfiguration configuration) : ITokenService
    {
        public string GenerateAccessToken(AdminUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.SerialNumber, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
            };

            var accessTokenMinutes = configuration.GetValue("Jwt:AccessTokenMinutes", 15);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(accessTokenMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public (string RawToken, RefreshToken Entity) GenerateRefreshToken(int adminUserId)
        {
            // The raw token goes to the browser; only its hash ever hits the DB.
            var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var refreshTokenDays = configuration.GetValue("Jwt:RefreshTokenDays", 7);

            var entity = new RefreshToken
            {
                TokenHash = HashRefreshToken(rawToken),
                ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDays),
                AdminUserId = adminUserId
            };

            return (rawToken, entity);
        }

        public string HashRefreshToken(string rawToken)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
            return Convert.ToHexString(bytes);
        }
    }
}
