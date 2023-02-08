namespace webapi.Controllers;

public class UserController : BaseApiController
{
    private readonly IUserService _userService;
    private readonly IAuthHelper _authHelper;
    private IDistributedCache _cache;

    public UserController(IUserService userService, IAuthHelper authHelper, IDistributedCache cache)
    {
        _userService = userService;
        _authHelper = authHelper;
        _cache = cache;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthenticateResDto>> Authenticate(AuthenticateReqDto dto)
    {
        try
        {
            return Ok(await _userService.AuthenticateAsync(dto));
        }
        catch (AppException ex)
        {
            return BadRequest(new ResponseMessage {Message = ex.Message});
        }
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterReqDto dto)
    {
        try
        {
            var user = await _userService.RegisterAsync(dto);
            return CreatedAtAction(nameof(GetUsersDetails), new {id = user.Id}, user);
        }
        catch (AppException ex)
        {
            return BadRequest(new ResponseMessage {Message = ex.Message});
        }
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("create-user")]
    public async Task<ActionResult> Create(RegisterReqDto dto)
    {
        try
        {
            var user = await _userService.CreateAsync(_authHelper.GetUserId(this), dto);
            return CreatedAtAction(nameof(GetUsersDetails), new {id = user.Id}, user);
        }
        catch (AppException ex)
        {
            return BadRequest(new ResponseMessage {Message = ex.Message});
        }
    }

    [HttpGet("users")]
    public async Task<ActionResult<PagedResult<GetAllResDto>>> GetUsers(
        [FromQuery] PaginationFilter paginationFilter)
    {
        return Ok(await _userService.GetAllAsync(paginationFilter));
    }

    [HttpGet("users/{id}")]
    public async Task<ActionResult<GetDetailsResDto>> GetUsersDetails(Guid id)
    {
        try
        {
            return Ok(await _userService.GetDetailsAsync(id));
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new ResponseMessage {Message = ex.Message});
        }
        catch (AppException ex)
        {
            return BadRequest(new ResponseMessage {Message = ex.Message});
        }
    }

    [HttpPatch("users/{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, UpdateReqDto dto)
    {
        try
        {
            var user = await _userService.UpdateAsync(id, _authHelper.GetUserId(this), dto);
            return CreatedAtAction(nameof(GetUsersDetails), new {id = user.Id}, user);
        }
        catch (ForbiddenException ex)
        {
            return StatusCode((int) HttpStatusCode.Forbidden, new ResponseMessage {Message = ex.Message});
        }
        catch (AppException ex)
        {
            return BadRequest(new ResponseMessage {Message = ex.Message});
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> DeleteAsync(Guid id)
    {
        try
        {
            await _userService.DeleteAsync(id, _authHelper.GetUserId(this));
            return NoContent();
        }
        catch (ForbiddenException ex)
        {
            return StatusCode((int) HttpStatusCode.Forbidden, new ResponseMessage {Message = ex.Message});
        }
    }
}