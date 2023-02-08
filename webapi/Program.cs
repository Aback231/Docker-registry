var builder = WebApplication.CreateBuilder(args);

// Setup NLog.
NLogBuilder.ConfigureNLog(builder.Environment.IsProduction() ? "NLog.config" : "NLog.Development.config");
builder.Host.UseNLog();

// Setup authorization.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole",
        policy => policy.RequireRole(webapi.Models.EnumerationTypes.Role.Admin.ToString()));
    options.AddPolicy("RequireUserRole",
        policy => policy.RequireRole(webapi.Models.EnumerationTypes.Role.User.ToString()));
    options.AddPolicy("RequireRootRole",
        policy => policy.RequireRole(webapi.Models.EnumerationTypes.Role.Root.ToString()));
});

// Select database depending on the current environment.
if (builder.Environment.IsProduction()) builder.Services.AddDbContext<AppDbContext>();
else builder.Services.AddDbContext<AppDbContext, DevAppDbContext>();
// Select PostgresDB
//builder.Services.AddDbContext<AppDbContext>();

// Redis DB
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisDB");
    //options.InstanceName = "Octopus_";
});

// Add controllers and Fluent validation.
builder.Services
    .AddControllers()
    .AddFluentValidation(configuration =>
    {
        configuration.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    });

// Configure strongly typed settings objects.
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);
var appSettings = appSettingsSection.Get<AppSettings>();

// Configure JWT authentication.
var key = Encoding.ASCII.GetBytes(appSettings.Secret);
builder.Services
    .AddAuthentication(configuration =>
    {
        configuration.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        configuration.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(configuration =>
    {
        configuration.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                var userId = context.Principal?.Identity?.Name;
                if (userId == null)
                {
                    context.Fail("Unauthorized");
                    return Task.CompletedTask;
                }

                var user = db.Users.AsNoTracking().Include(x => x.Role)
                    .FirstOrDefault(x => x.Id == Guid.Parse(userId));

                if (user == null)
                {
                    context.Fail("Unauthorized");
                    return Task.CompletedTask;
                }

                if (user.RoleId != null)
                {
                    var identity = context.Principal?.Identity as ClaimsIdentity;
                    identity?.AddClaim(new Claim(ClaimTypes.Role, user.Role.Name));
                }

                return Task.CompletedTask;
            }
        };
        configuration.RequireHttpsMetadata = false;
        configuration.SaveToken = true;
        configuration.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    })
    .AddCookie();

builder.Services.AddSingleton<IPasswordHasher<User>,PasswordHasher<User>>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Add Swagger.
builder.Services.AddSwaggerGen(configuration =>
{
    configuration.SwaggerDoc("v1", new OpenApiInfo { Title = "Web API", Version = "v1" });
    const string xmlFile = "ClassLibrary.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    configuration.IncludeXmlComments(xmlPath);
    configuration.CustomSchemaIds(type => type.ToString());
    configuration.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization header using the Bearer scheme.\r\n\r\n" +
            "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
            "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    configuration.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
});

builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();
builder.Services.AddCors();
builder.Services.AddLocalization();

// Configure DI for application services.
// Transient objects are always different; a new instance is provided to every controller and every service.
// Scoped objects are the same within a request, but different across different requests.
// Singleton objects are the same for every object and every request.
builder.Services.AddScoped<IPasswordHelper, PasswordHelper>();
builder.Services.AddScoped<IAuthHelper, AuthHelper>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDockerService, DockerService>();

var app = builder.Build();

if (builder.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

var logger = app.Services.GetService<ILogger>();

// Handle all uncaught exceptions. 
app.UseExceptionHandler(a => a.Run(
    async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature != null && logger != null)
        {
            var exception = exceptionHandlerPathFeature.Error;
            logger.LogError(exception, $"Unhandled exception caught by middleware: {exception.Message}");
            var result = JsonConvert.SerializeObject(new { error = exception.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync(result);
        }
    }));

// Get Main DB Context
using var serviceScope = app.Services.GetService<IServiceScopeFactory>()?.CreateScope();

if (serviceScope == null) throw new ApplicationException("Cannot create service scope.");

var db = serviceScope.ServiceProvider.GetService<AppDbContext>();
var passwordHelper = serviceScope.ServiceProvider.GetService<IPasswordHelper>();
if (db == null || passwordHelper == null) throw new ApplicationException("Cannot get service.");

// Migrate any database changes on startup (includes initial database creation) and Seed admin user and roles.
await db.Database.MigrateAsync();

if (db.Users.FirstOrDefault(x => x.Username == appSettings.AdminUsername) == null)
{
    var (passwordHash, passwordSalt) = passwordHelper.CreateHash(appSettings.AdminPassword);

    var user = new User
    {
        Id = Guid.NewGuid(),
        Username = appSettings.AdminUsername,
        Email = appSettings.AdminEmail,
        CreatedAt = DateTime.UtcNow,
        IsActive = true,
        PasswordHash = passwordHash,
        PasswordSalt = passwordSalt
    };

    db.Users.Add(user);
    db.SaveChanges();

    foreach (var role in Enum.GetNames(typeof(webapi.Models.EnumerationTypes.Role)))
    {
        var db_role = new Role
        {
            Id = Guid.NewGuid(),
            Name = role.ToString(),
            CreatedById = user.Id
        };
        if (db_role.Name == webapi.Models.EnumerationTypes.Role.Admin.ToString())
            user.RoleId = db_role.Id;
        db.Roles.Add(db_role);
    }

    db.SaveChanges();
}

// The localization middleware must be configured before
// any middleware which might check the request culture.
var supportedCultures = new[]
{
    new CultureInfo("en"),
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseRouting();

// Set global CORS policy.
app.UseCors(configuration => configuration.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapHealthChecks("/health");
    }
);

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API v1");
    c.DisplayRequestDuration();
});

app.Run();