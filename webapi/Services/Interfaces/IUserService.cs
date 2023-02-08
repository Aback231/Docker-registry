namespace webapi.Services.Interfaces;

public interface IUserService
{
    Task<AuthenticateResDto> AuthenticateAsync(AuthenticateReqDto dto);
    Task<GetDetailsResDto> RegisterAsync(RegisterReqDto dto);
    Task<GetDetailsResDto> CreateAsync(Guid userId, RegisterReqDto dto);
    Task<PagedResult<GetAllResDto>> GetAllAsync(PaginationFilter paginationFilter);
    Task<GetDetailsResDto> GetDetailsAsync(Guid id);
    Task<GetDetailsResDto> UpdateAsync(Guid id, Guid userId, UpdateReqDto dto);
    Task DeleteAsync(Guid id, Guid userId);
}