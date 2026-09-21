using PayRollApi.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PayRollApi.Application.Interfaces
{
    public interface ICRUDinterface<T> where T : class, IAuditable
    {
        Task<ICollection<T>> GetAll();
        Task<ICollection<T>> GetPages(int currentpage = 0, int pageSize = 100);
        Task<T> GetById(int id);
        Task<T> Create(T entity);
        Task<List<T>> CreateRange(List<T> entity);
        Task Update(T entity); 
        Task<bool> Delete(int id);
        Task<bool> DeleteRange(List<T> values);
        Task UpdateRange(List<T> entity);
        Task SaveChangesAsync();
        Task<List<T>> GetDynamic(string tableName, string? condition = null);
    }
}
