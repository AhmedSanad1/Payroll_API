using PayRollApi.Application.Interfaces.UOWInterfaces;
using PayRollApi.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace PayRollApi.Infrastructure.Persistence.UOWs
{
    public class BaseUOW(PayRollDbContext context) : IBaseUOW
    {
        public async Task<int> CompleteAsync()
        {
            return await context.SaveChangesAsync();
        }
    }
}
