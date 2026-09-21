using System;
using System.Collections.Generic;
using System.Text;

namespace PayRollApi.Application.Interfaces.UOWInterfaces
{
    public interface IBaseUOW
    {
        Task<int> CompleteAsync();

    }
}
