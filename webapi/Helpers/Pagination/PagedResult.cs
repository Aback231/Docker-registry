using System;
using System.Collections.Generic;

namespace webapi.Helpers.Pagination;

public class PagedResult<T> where T : class
{
    public int CurrentPage { get; set; }
    public int PageCount { get; set; }
    public int PageSize { get; set; }
    public int RowCount { get; set; }
    public int FirstRowReturned => (CurrentPage - 1) * PageSize + 1;
    public int LastRowReturned => Math.Min(CurrentPage * PageSize, RowCount);
    public IList<T> Data { get; set; }
    public PagedResult()
    {
        Data = new List<T>();
    }
}