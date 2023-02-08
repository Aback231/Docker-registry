namespace webapi.Controllers;

public class DockerController : BaseApiController
{
    private readonly IUserService _userService;
    private readonly IDockerService _dockerService;
    private readonly IAuthHelper _authHelper;
    private IDistributedCache _cache;
    private readonly AppDbContext _db;

    public DockerController(AppDbContext db, IUserService userService, IAuthHelper authHelper, IDistributedCache cache, IDockerService dockerService)
    {
        _userService = userService;
        _authHelper = authHelper;
        _dockerService = dockerService;
        _cache = cache;
        _db = db;
    }

    [Authorize]
    [HttpGet("docker/user-containers")]
    public async Task<ActionResult<DockerContainerDetailResDto>> ListUserContainers()
    {
        var currentUser = await _db.Users.SingleOrDefaultAsync(x => x.Id == _authHelper.GetUserId(this));

        var userContainers = _db.DockerContainer
            .Where(x => x.UserId == currentUser.Id)
            .Select(x => new DockerContainerDetailResDto
            {
                Id = x.Id,
                RepositoryName = x.RepositoryName,
                Tag = x.Tag,
                Hash = x.Hash,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                UserId = x.UserId
            }).ToList();
        
        return Ok(userContainers);
    }

    [Authorize]
    [HttpGet("docker/user-images")]
    public async Task<ActionResult<DockerImageDetailResDto>> ListUserImages()
    {
        var currentUser = await _db.Users.SingleOrDefaultAsync(x => x.Id == _authHelper.GetUserId(this));

        var userImages = _db.DockerImage
            .Where(x => x.UserId == currentUser.Id)
            .Select(x => new DockerImageDetailResDto
            {
                Id = x.Id,
                RepositoryName = x.RepositoryName,
                Tag = x.Tag,
                Hash = x.Hash,
                CurrentVersion = x.CurrentVersion,
                NewestVersion = x.NewestVersion,
                NewestVersionPulled = x.NewestVersionPulled,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                UserId = x.UserId
            }).ToList();
        
        return Ok(userImages);
    }

    [Authorize]
    [HttpPost("docker/container-add")]
    public async Task<ActionResult<DockerContainerResDto>> AddContainer(DockerContainerReqDto dockerContainerReqDt)
    {
        var currentUser = await _db.Users.SingleOrDefaultAsync(x => x.Id == _authHelper.GetUserId(this));

        var newDockerContainer = new DockerContainer()
        {
            Id = Guid.NewGuid(),
            RepositoryName = dockerContainerReqDt.RepositoryName,
            Tag = dockerContainerReqDt.Tag,
            Hash = dockerContainerReqDt.Hash,
            ExposedPort = dockerContainerReqDt.ExposedPort,
            Volume = dockerContainerReqDt.Volume,
            SocketBind = dockerContainerReqDt.SocketBind,
            ContainerPort = dockerContainerReqDt.ContainerPort,
            HostPort = dockerContainerReqDt.HostPort,
            User = currentUser,
            CreatedAt = DateTime.UtcNow,
        };
        
        await _dockerService.AddContainerAsync(newDockerContainer);

        return new DockerContainerResDto()
        {
            Status = "true"
        };
    }

    [Authorize]
    [HttpPost("docker/container-update")]
    public async Task<ActionResult<DockerContainerResDto>> UpdateContainer(DockerContainerUpdateReqDto dockerContainerUpdateReqDto)
    {
        var container = await _db.DockerContainer.SingleOrDefaultAsync(x => x.Id == dockerContainerUpdateReqDto.Id);

        container.Tag = dockerContainerUpdateReqDto.Tag;
        container.Hash = dockerContainerUpdateReqDto.Hash;
        container.UpdatedAt = DateTime.UtcNow;
        
        await _dockerService.UpdateContainerAsync(container);

        return new DockerContainerResDto()
        {
            Status = "true"
        };
    }

    [Authorize]
    [HttpPost("docker/image-add")]
    public async Task<ActionResult<DockerImageResDto>> AddImage(DockerImageReqDto dockerImageReqDto)
    {
        var currentUser = await _db.Users.SingleOrDefaultAsync(x => x.Id == _authHelper.GetUserId(this));

        var newDockerImage = new DockerImage()
        {
            Id = Guid.NewGuid(),
            RepositoryName = dockerImageReqDto.RepositoryName,
            Tag = dockerImageReqDto.Tag,
            User = currentUser,
            CreatedAt = DateTime.UtcNow,
        };
        
        await _dockerService.AddImageAsync(newDockerImage);

        return new DockerImageResDto()
        {
            Status = "true"
        };
    }

    [Authorize]
    [HttpPost("docker/image-update")]
    public async Task<ActionResult<DockerImageResDto>> UpdateImage(DockerImageUpdateReqDto dockerImageUpdateReqDto)
    {
        var image = await _db.DockerImage.SingleOrDefaultAsync(x => x.Id == dockerImageUpdateReqDto.Id);

        image.Tag = dockerImageUpdateReqDto.Tag;
        image.Hash = dockerImageUpdateReqDto.Hash;
        image.CurrentVersion = dockerImageUpdateReqDto.CurrentVersion;
        image.NewestVersion = dockerImageUpdateReqDto.NewestVersion;
        image.NewestVersionPulled = dockerImageUpdateReqDto.NewestVersionPulled;
        image.UpdatedAt = DateTime.UtcNow;
        
        await _dockerService.UpdateImageAsync(image);

        return new DockerImageResDto()
        {
            Status = "true"
        };
    }
}