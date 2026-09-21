using Microsoft.EntityFrameworkCore;
using PayRollApi.Application.Interfaces;
using PayRollApi.Domain.Entities.SecurityModule;
using PayRollApi.Infrastructure.Persistence.Contexts;

namespace PayRollApi.Infrastructure.Persistence.Repositories
{
    public class GetUserRepository(PayRollDbContext dbContext) : IGetUser
    {
        public Task<AdminUser?> GetByUsername(string username)
        {
            return dbContext.AdminUsers.FirstOrDefaultAsync(user => user.Username == username);
        }
    }
}
