
using PayRollApi.Application.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayRollApi.Infrastructure.Helper
{
    public static class PagersExt
    {
        // Only reached through ICRUDinterface.GetPages, which no feature calls yet. Careful:
        // currentpage is 0-based here but PageList is 1-based, so the page metadata lies.
        public async static Task<PageList<T>> Pagination<T>(this IQueryable<T> query,int currentpage =0, int pageSize =100) where T : class
        {
            var countitem = query.Count();

            query = query.Skip(currentpage * (pageSize == 0 ? 100 : pageSize));
            if (pageSize > 0)
            {
                query = query.Take((int)pageSize);
            }
            var items = await query.ToListAsync();

            return new PageList<T>(items, countitem, currentpage, pageSize);
        }
    }
}
