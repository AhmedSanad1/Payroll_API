using PayRollApi.Application.Common;
using PayRollApi.Application.DTOs;

namespace PayRollApi.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResult>> Login(LoginRequest request);

        // A bad or missing token is normal here (the SPA tries this on every load) — 401, not an exception.
        Task<ApiResponse<AuthResult>> Refresh(string? rawRefreshToken);

        Task Logout(string? rawRefreshToken);
        Task<ApiResponse<CurrentUserDto>> Me(int adminUserId);
    }
}
