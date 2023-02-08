namespace dockerapi.Services.Interfaces;

public interface IDockerService
{
    Task<IList<ImagesListResponse>> ListImagesAsync(CancellationToken cancellationToken);
    Task<IList<ContainerListResponse>> ListContainersAsync(CancellationToken cancellationToken);
    Task<DockerImageResDto> PullImageAsync(DockerImageReqDto dockerImageReqDto, CancellationToken cancellationToken);
    Task<DockerContainerStartResDto> ContainerStartAsync(DockerContainerReqDto dockerContainerReqDto, CancellationToken cancellationToken, ContainerListResponse c = default);
    Task<DockerContainerStopResDto> ContainerStopAsync(DockerContainerReqDto dockerContainerReqDto, IList<ContainerListResponse> runningContainers, CancellationToken cancellationToken);
    DockerContainerStatsResDto ContainerStats(DockerContainerReqDto dockerContainerReqDto, IList<ContainerListResponse> runningContainers, CancellationToken cancellationToken);
}