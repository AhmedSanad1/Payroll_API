using PayRollApi.Domain.Entities.SecurityModule;

namespace PayRollApi.Application.Interfaces
{
    public interface IGetUser
    {
        Task<AdminUser?> GetByUsername(string username);
    }
}
