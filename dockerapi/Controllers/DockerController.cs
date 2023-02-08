namespace dockerapi.Controllers;

public class DockerController : BaseApiController
{
    private readonly IDockerService _dockerService;
    public DockerController(ILogger<DockerController> logger, IDockerService dockerService)
    {
        _dockerService = dockerService;
    }

    [HttpGet("list-images")]
    public async Task<ActionResult> GetListImages(CancellationToken cancellationToken)
    {
        var images = await _dockerService.ListImagesAsync(cancellationToken);

        return Ok(images);
    }

    [HttpGet("list-containers")]
    public async Task<ActionResult> ListContainers(CancellationToken cancellationToken)
    {
        var containers = await _dockerService.ListContainersAsync(cancellationToken);

        return Ok(containers);
    }

    // Pull Image from private or public repository
    [HttpPost("image-pull")]
    public async Task<ActionResult<DockerImageResDto>> ImagePull(DockerImageReqDto dockerImageReqDto, CancellationToken cancellationToken)
    {
        var imageStatus = await _dockerService.PullImageAsync(dockerImageReqDto, cancellationToken);

        return Ok(imageStatus);
    }

    // Multiple containers of the same Image can be started with different ID's
    [HttpPost("container-start")]
    public async Task<ActionResult<DockerContainerStartResDto>> ContainerStart(DockerContainerReqDto dockerContainerStartReqDto, CancellationToken cancellationToken)
    {
        var startedContainer = await _dockerService.ContainerStartAsync(dockerContainerStartReqDto, cancellationToken);

        return Ok(startedContainer);
    }

    [HttpPost("containers-stop")]
    public async Task<ActionResult<DockerContainerStopResDto>> ContainersStop(DockerContainerReqDto dockerContainerReqDto, CancellationToken cancellationToken)
    {
        var runningContainers = await _dockerService.ListContainersAsync(cancellationToken);
        var stoppedContainers = await _dockerService.ContainerStopAsync(dockerContainerReqDto, runningContainers, cancellationToken);

        return Ok(stoppedContainers);
    }

    [HttpPost("container-update")]
    public async Task<ActionResult<DockerContainerUpdateResDto>> ContainerUpdate(DockerContainerReqDto dockerContainerReqDto, CancellationToken cancellationToken)
    {
        var runningContainer = new ContainerListResponse();
        var stoppedContainers = new DockerContainerStopResDto();
        var startedContainer = new DockerContainerStartResDto();
        var imageName = dockerContainerReqDto.RepositoryName + ":" + dockerContainerReqDto.Tag;

        var runningContainers = await _dockerService.ListContainersAsync(cancellationToken);

        foreach (var c in runningContainers)
        {
            if (c.Image == dockerContainerReqDto.RepositoryName + ":" + dockerContainerReqDto.Tag)
                runningContainer = c;
            break;
        }

        // Docker controll API will kill itself unless new escaped container is created at first
        
        /// That creates a Port binding problem as we try to bind to 2 ports at the same time on the same network
        
        //// Can be solved using NGINX as loadbalancer, which would make scaling possible, and eliminate the
        //// need for port binding. All that must however be done on real network because it will not work
        //// on localhost (I've tried)!
        //// In mem DB like REDIS could also solve this priblem. The new container could be bound to current port
        //// + 1, we save data in REDIS and upon launch we deal with the old container, and restart the main
        //// one to desired port. This will introduce some latency.

        ///// This can also be potentiallly solved by usin docker isolated VLAN taging network which 
        ///// docker supports, as it has posibillity to bind on 2 ports at the same time! This will probably
        ///// be tricky to acomplish with current C# docker lib which is far from official ones for Python and GO
        if (imageName.Contains("dockerapi"))
        {
            startedContainer = await _dockerService.ContainerStartAsync(dockerContainerReqDto, cancellationToken, runningContainer);
            if (startedContainer.Status)
                stoppedContainers = await _dockerService.ContainerStopAsync(dockerContainerReqDto, runningContainers, cancellationToken);
        }
        else
        {
            stoppedContainers = await _dockerService.ContainerStopAsync(dockerContainerReqDto, runningContainers, cancellationToken);
            startedContainer = await _dockerService.ContainerStartAsync(dockerContainerReqDto, cancellationToken, runningContainer);
        }

        return new DockerContainerUpdateResDto
        {
            Status = startedContainer.Status,
            CreateContainer = startedContainer.CreateContainer,
            StopContainers = stoppedContainers.StopContainers
        };
    }

    [HttpPost("containers-stats")]
    public async Task<ActionResult<DockerContainerStatsResDto>> ContainersStats(DockerContainerReqDto dockerContainerReqDto, CancellationToken cancellationToken)
    {
        var runningContainers = await _dockerService.ListContainersAsync(cancellationToken);
        var runningContainersStats = _dockerService.ContainerStats(dockerContainerReqDto, runningContainers, cancellationToken);

        return Ok(runningContainersStats);
    }
}
