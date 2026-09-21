using PayRollApi.Application.Interfaces;
using PayRollApi.Application.Interfaces.UOWInterfaces;
using PayRollApi.Domain.Entities.SecurityModule;
using PayRollApi.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace PayRollApi.Infrastructure.Persistence.UOWs
{
    public class TokenUOW(PayRollDbContext context,
        IServiceProvider serviceProvider) : BaseUOW(context), ITokenUOW
    {
        public ICRUDinterface<AdminUser> UsersCRUD => serviceProvider.GetService<ICRUDinterface<AdminUser>>()!;
        public IRefreshTokenCRUD RefreshTokenCRUD => serviceProvider.GetService<IRefreshTokenCRUD>()!;
    }
}
