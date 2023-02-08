namespace webapi.Data;

public class AppDbContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public AppDbContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Configuration.GetConnectionString("webapiDatabase"));
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<DockerContainer> DockerContainer { get; set; }
    public DbSet<DockerImage> DockerImage { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("users");

            // Property.
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.Username).IsRequired().HasMaxLength(255);
            b.Property(x => x.GivenName).HasMaxLength(30);
            b.Property(x => x.FamilyName).HasMaxLength(30);
            b.Property(x => x.Email).HasMaxLength(320);
            b.Property(x => x.CreatedAt).IsRequired();

            // Index.
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.Username);
            b.HasIndex(x => x.IsActive);
            b.HasIndex(x => x.RoleId);
            b.HasIndex(x => x.CreatedById);
            b.HasIndex(x => x.UpdatedById);

            // Relation.
            b.HasOne(x => x.Role).WithMany(y => y.Users).HasForeignKey(z => z.RoleId);
            b.HasOne(x => x.CreatedBy).WithMany(x => x.CreatedUsers).HasForeignKey(x => x.CreatedById);
            b.HasOne(x => x.UpdatedBy).WithMany(x => x.UpdatedUsers).HasForeignKey(x => x.UpdatedById);

            b.HasMany(x => x.DockerContainers).WithOne(y => y.User).HasForeignKey(x => x.UserId);
            b.HasMany(x => x.DockerImages).WithOne(y => y.User).HasForeignKey(x => x.UserId);
        });

        modelBuilder.Entity<Role>(b =>
        {
            b.ToTable("roles");

            // Property.
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.Name).IsRequired().HasMaxLength(64);

            // Index.
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.Name).IsUnique();
            b.HasIndex(x => x.CreatedById);
            b.HasIndex(x => x.UpdatedById);

            // Relation.
            b.HasOne(x => x.CreatedBy).WithMany(x => x.CreatedRoles).HasForeignKey(x => x.CreatedById);
            b.HasOne(x => x.UpdatedBy).WithMany(x => x.UpdatedRoles).HasForeignKey(x => x.UpdatedById);
        });

        modelBuilder.Entity<DockerContainer>(b =>
        {
            b.ToTable("docker_container");

            // Property.
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.RepositoryName).HasMaxLength(264);
            b.Property(x => x.Tag).HasMaxLength(264);
            b.Property(x => x.CreatedAt).IsRequired();

            // Index.
            b.HasKey(x => x.Id);
        });

        modelBuilder.Entity<DockerImage>(b =>
        {
            b.ToTable("docker_image");

            // Property.
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.RepositoryName).HasMaxLength(264);
            b.Property(x => x.Tag).HasMaxLength(264);
            b.Property(x => x.CreatedAt).IsRequired();

            // Index.
            b.HasKey(x => x.Id);
        });

        modelBuilder.Entity<Log>().ToTable("logs");
    }
}