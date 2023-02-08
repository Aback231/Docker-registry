namespace webapi.Services.Interfaces;

public interface IDockerService
{
    Task AddContainerAsync(DockerContainer dockerContainer);
    Task UpdateContainerAsync(DockerContainer dockerContainer);
    Task AddImageAsync(DockerImage dockerImage);
    Task UpdateImageAsync(DockerImage dockerImage);
}