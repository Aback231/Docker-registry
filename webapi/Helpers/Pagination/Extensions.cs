using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace webapi.Helpers.Pagination;

public static class Extensions
{
    public static async Task<PagedResult<T>> GetPagedAsync<T>(this IQueryable<T> query, PaginationFilter pf) where
        T : class
    {
        var result = new PagedResult<T>
        {
            CurrentPage = pf.PageNumber,
            PageSize = pf.PageSize,
            RowCount = query.Count()
        };
            
        var pageCount = (double) result.RowCount / pf.PageSize;
        result.PageCount = (int) Math.Ceiling(pageCount);

        var skip = (pf.PageNumber - 1) * pf.PageSize;
        result.Data = await query.Skip(skip).Take(pf.PageSize).ToListAsync();

        return result;
    }
}