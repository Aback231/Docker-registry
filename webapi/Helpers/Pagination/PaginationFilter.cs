namespace webapi.Helpers.Pagination;

public class PaginationFilter
{
    private int _pageNumber;
    private int _pageSize;
    private readonly int _maxPageSize = 25;

    public int PageNumber
    {
        get => _pageNumber == 0 ? 1 : _pageNumber;
        set => _pageNumber = value;
    }

    public int PageSize
    {
        get => _pageSize > _maxPageSize || _pageSize == 0 ? _maxPageSize : _pageSize;
        set => _pageSize = value;
    }

    public PaginationFilter()
    {
    }

    public PaginationFilter(int maxPageSize)
    {
        _maxPageSize = maxPageSize;
    }
}