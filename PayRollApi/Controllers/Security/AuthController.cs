using PayRollApi.Application.Common;
using PayRollApi.Application.DTOs;
using PayRollApi.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace PayRollApi.Controllers.Security
{
    // Keep this lowercase — the refresh cookie is scoped to /api/auth and browsers match cookie paths case-sensitively.
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService, IUserService userService) : ControllerBase
    {
        private const string RefreshTokenCookieName = "refreshToken";

        [HttpPost("[action]")]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await authService.Login(request);

            if (result.Object is null)
                return StatusCode((int)result.StatusCode, new ApiResponse<object>
                {
                    StatusCode = result.StatusCode,
                    Message = result.Message,
                    Errors = result.Errors
                });

            SetRefreshTokenCookie(result.Object.RawRefreshToken, result.Object.RefreshTokenExpiresAtUtc);

            return Ok(new ApiResponse<LoggedUser>
            {
                StatusCode = result.StatusCode,
                Message = result.Message,
                Object = ToLoggedUser(result.Object)
            });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Refresh()
        {
            var rawRefreshToken = Request.Cookies[RefreshTokenCookieName];
            var result = await authService.Refresh(rawRefreshToken);

            if (result.Object is null)
            {
                // Whatever the browser sent is dead — drop it so it stops being re-sent on every page load.
                DeleteRefreshTokenCookie();
                return StatusCode((int)result.StatusCode, new ApiResponse<object>
                {
                    StatusCode = result.StatusCode,
                    Message = result.Message,
                    Errors = result.Errors
                });
            }

            SetRefreshTokenCookie(result.Object.RawRefreshToken, result.Object.RefreshTokenExpiresAtUtc);

            return Ok(new ApiResponse<LoggedUser>
            {
                StatusCode = result.StatusCode,
                Message = result.Message,
                Object = ToLoggedUser(result.Object)
            });
        }

        [HttpPost("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Logout()
        {
            var rawRefreshToken = Request.Cookies[RefreshTokenCookieName];
            await authService.Logout(rawRefreshToken);
            DeleteRefreshTokenCookie();
            return Ok(new SuccessResponse<object>("Success"));
        }

        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Me()
        {
            var userId = userService.GetUserId();
            if (userId is null)
                return Unauthorized();

            var result = await authService.Me(userId.Value);
            return StatusCode((int)result.StatusCode, result);
        }

        private static LoggedUser ToLoggedUser(AuthResult result) =>
            new(result.AccessToken, result.AccessTokenExpiresAtUtc, result.Username);

        private void DeleteRefreshTokenCookie() =>
            Response.Cookies.Delete(RefreshTokenCookieName, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/api/auth"
            });

        private void SetRefreshTokenCookie(string rawRefreshToken, DateTime expiresAtUtc)
        {
            // HttpOnly so JS can never read the refresh token.
            Response.Cookies.Append(RefreshTokenCookieName, rawRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/api/auth",
                Expires = expiresAtUtc
            });
        }
    }
}
