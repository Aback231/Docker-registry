namespace webapi.Services;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly AppDbContext _db;
    private readonly AppSettings _appSettings;
    private readonly IStringLocalizer<Translation> _l;
    private readonly IStringLocalizer<LocalizedResource> _lr;
    private readonly IPasswordHelper _passwordHelper;
    private readonly EmbeddedFileProvider _embedded;
    private readonly IAuthHelper _authHelper;

    public UserService(ILogger<UserService> logger, AppDbContext db, IOptions<AppSettings> appSettings,
        IStringLocalizer<Translation> l, IStringLocalizer<LocalizedResource> lr, IPasswordHelper passwordHelper,
        IAuthHelper authHelper)
    {
        _logger = logger;
        _db = db;
        _appSettings = appSettings.Value;
        _l = l;
        _lr = lr;
        _passwordHelper = passwordHelper;
        _authHelper = authHelper;
        _embedded = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
    }

    public async Task<AuthenticateResDto> AuthenticateAsync(AuthenticateReqDto dto)
    {
        var user = await _db.Users.SingleOrDefaultAsync(x => x.Username == dto.Username && x.IsActive);

        // Check if the username exists.
        if (user == null) throw new EntityNotFoundException(_l["Username is incorrect."]);

        // Check for too many failed login attempts.
        if (user.LoginFailedAt != null)
        {
            var secondsPassed = DateTime.UtcNow.Subtract(user.LoginFailedAt.GetValueOrDefault()).Seconds;

            var isMaxCountExceeded = user.LoginFailedCount >= _appSettings.MaxLoginFailedCount;
            var isWaitingTimePassed = secondsPassed > _appSettings.LoginFailedWaitingTime;

            if (isMaxCountExceeded && !isWaitingTimePassed)
            {
                var secondsToWait = _appSettings.LoginFailedWaitingTime - secondsPassed;
                throw new TooManyFailedLoginAttemptsException(string.Format(
                    _l["You must wait for {0} seconds before you try to log in again."], secondsToWait));
            }
        }

        // Check if password is correct.
        if (!_passwordHelper.VerifyHash(dto.Password, user.PasswordHash, user.PasswordSalt))
        {
            user.LoginFailedCount += 1;
            user.LoginFailedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            throw new IncorrectPasswordException(_l["Password is incorrect."]);
        }

        // Authentication successful.
        user.LoginFailedCount = 0;
        user.LoginFailedAt = null;
        user.LastLoginAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return new AuthenticateResDto
        {
            Id = Guid.NewGuid(),
            Username = user.Username,
            GivenName = user.GivenName,
            FamilyName = user.FamilyName,
            Email = user.Email,
            Token = CreateToken(user.Id.ToString())
        };
    }

    public async Task<GetDetailsResDto> RegisterAsync(RegisterReqDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new InvalidPasswordException(_l["Password is required."]);

        var existingUser = await _db.Users.FirstOrDefaultAsync(
            x => x.Username == dto.Username || x.Email == dto.Email);

        if (existingUser?.Username == dto.Username)
            throw new UsernameTakenException(string.Format(_l["Username '{0}' is already taken."], dto.Username));

        // if (existingUser?.Email == dto.Email)
        //     throw new EmailTakenException(string.Format(_l["Email '{0}' is already taken."], dto.Email));

        var (passwordHash, passwordSalt) = _passwordHelper.CreateHash(dto.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            GivenName = dto.GivenName,
            FamilyName = dto.FamilyName,
            Username = dto.Username,
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            RoleId = _db.Roles.Single(b => b.Name == webapi.Models.EnumerationTypes.Role.User.ToString()).Id,
            IsActive = true
        };

        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();

        return new GetDetailsResDto
        {
            Id = Guid.NewGuid(),
            Username = user.Username,
            GivenName = user.GivenName,
            FamilyName = user.FamilyName,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            LastLoginAt = user.LastLoginAt,
            Role = new RoleResDto {Id = user.Role.Id, Name = user.Role.Name},
            IsActive = user.IsActive
        };
    }

    public async Task<GetDetailsResDto> CreateAsync(Guid userId, RegisterReqDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new InvalidPasswordException(_l["Password is required."]);

        var existingUser = await _db.Users.FirstOrDefaultAsync(
            x => x.Username == dto.Username || x.Email == dto.Email);

        if (existingUser?.Username == dto.Username)
            throw new UsernameTakenException(string.Format(_l["Username '{0}' is already taken."], dto.Username));

        if (existingUser?.Email == dto.Email)
            throw new EmailTakenException(string.Format(_l["Email '{0}' is already taken."], dto.Email));

        var (passwordHash, passwordSalt) = _passwordHelper.CreateHash(dto.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            GivenName = dto.GivenName,
            FamilyName = dto.FamilyName,
            Username = dto.Username,
            IsActive = true,
            Email = dto.Email,
            CreatedById = userId,
            CreatedAt = DateTime.UtcNow,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();

        return new GetDetailsResDto
        {
            Id = Guid.NewGuid(),
            Username = user.Username,
            GivenName = user.GivenName,
            FamilyName = user.FamilyName,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            LastLoginAt = user.LastLoginAt,
            IsActive = user.IsActive
        };
    }

    public async Task<PagedResult<GetAllResDto>> GetAllAsync(PaginationFilter paginationFilter)
    {
        return await _db.Users.AsNoTracking()
            .Include(x => x.Role)
            .Where(x => x.IsActive)
            .Select(x => new GetAllResDto
            {
                Id = x.Id,
                FirstName = x.GivenName,
                LastName = x.FamilyName,
                Username = x.Username,
                Email = x.Email,
                Role = x.Role == null
                    ? null
                    : new RoleResDto {Id = x.Role.Id, Name = x.Role.Name}
            })
            .GetPagedAsync(paginationFilter);
    }

    public async Task<GetDetailsResDto> GetDetailsAsync(Guid id)
    {
        var user = await _db.Users.AsNoTracking()
            .Include(x => x.Role)
            .Where(x => x.Id == id)
            .Select(x => new GetDetailsResDto
            {
                Id = x.Id,
                Username = x.Username,
                GivenName = x.GivenName,
                FamilyName = x.FamilyName,
                Email = x.Email,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                LastLoginAt = x.LastLoginAt,
                IsActive = x.IsActive,
                Role = x.Role == null
                    ? null
                    : new RoleResDto
                    {
                        Id = x.Role.Id, Name = x.Role.Name
                    }
            })
            .FirstOrDefaultAsync();
        if (user == null) throw new EntityNotFoundException(_l["User not found."]);
        return user;
    }

    public async Task<GetDetailsResDto> UpdateAsync(Guid id, Guid userId,
        UpdateReqDto dto)
    {
        if (userId != id) throw new ForbiddenException();

        var user = await _db.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Id == id);
        if (user == null) throw new EntityNotFoundException(_l["User not found."]);

        // Update username if it has changed.
        if (dto.Username != null && dto.Username.NewValue != user.Username)
        {
            if (_db.Users.Any(x => x.Username == dto.Username.NewValue))
                throw new UsernameTakenException(string.Format(
                    _l["Username '{0}' is already taken."], dto.Username.NewValue));
            user.Username = dto.Username.NewValue;
        }

        // Update user properties if provided.
        if (dto.GivenName != null)
        {
            user.GivenName = dto.GivenName.NewValue;
        }

        if (dto.FamilyName != null)
        {
            user.FamilyName = dto.FamilyName.NewValue;
        }

        // Update password if provided.
        if (dto.Password != null)
        {
            var (passwordHash, passwordSalt) = _passwordHelper.CreateHash(dto.Password.NewValue);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
        }

        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedById = userId;

        await _db.SaveChangesAsync();

        return new GetDetailsResDto
        {
            Id = Guid.NewGuid(),
            Username = user.Username,
            GivenName = user.GivenName,
            FamilyName = user.FamilyName,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            LastLoginAt = user.LastLoginAt,
            IsActive = user.IsActive,
            Role = user.Role == null
                ? null
                : new RoleResDto
                {
                    Id = user.Role.Id, Name = user.Role.Name
                }
        };
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        if (userId != id) throw new ForbiddenException();

        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user != null)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }
    }

    public string CreateToken(string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, userId)}),
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}