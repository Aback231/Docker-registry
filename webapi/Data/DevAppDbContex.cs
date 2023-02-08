namespace webapi.Data;

public class DevAppDbContext : AppDbContext
{
    private static readonly ILoggerFactory DbLoggerFactory = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
    });

    public DevAppDbContext(IConfiguration configuration) : base(configuration)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseLoggerFactory(DbLoggerFactory)
            .UseSqlite(Configuration.GetConnectionString("webapiDatabase"));
    }
}