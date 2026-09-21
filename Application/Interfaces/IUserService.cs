using System.Security.Claims;

namespace PayRollApi.Application.Interfaces
{
    public interface IUserService
    {
        ClaimsPrincipal? GetUser();
        int? GetUserId();
    }
}
