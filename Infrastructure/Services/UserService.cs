using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using PayRollApi.Application.Interfaces;

namespace PayRollApi.Infrastructure.Services
{
    public class UserService(IHttpContextAccessor accessor) : IUserService
    {
        public ClaimsPrincipal? GetUser()
        {
            return accessor.HttpContext?.User;
        }

        public int? GetUserId()
        {
            var value = accessor.HttpContext?.User.FindFirst(ClaimTypes.SerialNumber)?.Value;
            return int.TryParse(value, out var id) ? id : null;
        }
    }
}
