namespace dockerapi.Extensions

{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            DockerClient _dockerClient = new DockerClientConfiguration().CreateClient();
            services.AddSingleton<DockerClient>(_dockerClient);

            return services;
        }
    }
}