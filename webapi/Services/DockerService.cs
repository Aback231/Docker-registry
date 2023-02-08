namespace webapi.Services;

public class DockerService : IDockerService
{
    private readonly ILogger<DockerService> _logger;
    private readonly AppDbContext _db;
    private readonly AppSettings _appSettings;
    private readonly IStringLocalizer<Translation> _l;
    private readonly IStringLocalizer<LocalizedResource> _lr;
    private readonly IPasswordHelper _passwordHelper;
    private readonly EmbeddedFileProvider _embedded;

    public DockerService(ILogger<DockerService> logger, AppDbContext db, IOptions<AppSettings> appSettings,
        IStringLocalizer<Translation> l, IStringLocalizer<LocalizedResource> lr, IPasswordHelper passwordHelper)
    {
        _logger = logger;
        _db = db;
        _appSettings = appSettings.Value;
        _l = l;
        _lr = lr;
        _passwordHelper = passwordHelper;
        _embedded = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
    }

    public async Task AddContainerAsync(DockerContainer dockerContainer)
    {
        _db.DockerContainer.Add(dockerContainer);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateContainerAsync(DockerContainer dockerContainer)
    {
        _db.Update(dockerContainer);
        await _db.SaveChangesAsync();
    }

    public async Task AddImageAsync(DockerImage dockerImage)
    {
        _db.DockerImage.Add(dockerImage);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateImageAsync(DockerImage dockerImage)
    {
        _db.Update(dockerImage);
        await _db.SaveChangesAsync();
    }

}