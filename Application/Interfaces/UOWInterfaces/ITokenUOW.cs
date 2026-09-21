using PayRollApi.Domain.Entities.SecurityModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace PayRollApi.Application.Interfaces.UOWInterfaces
{
    public interface ITokenUOW
    {
        ICRUDinterface<AdminUser> UsersCRUD { get; }
        IRefreshTokenCRUD RefreshTokenCRUD { get; }
        Task<int> CompleteAsync();
    }
}
