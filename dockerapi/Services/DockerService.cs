using Newtonsoft.Json;

namespace dockerapi.Services;

public class DockerService : IDockerService
{
    private readonly DockerClient _dockerClient;
    private readonly IMapper _mapper;

    public DockerService(DockerClient dockerClient, IMapper mapper)
    {
        _dockerClient = dockerClient;
        _mapper = mapper;
    }

    public async Task<IList<ImagesListResponse>> ListImagesAsync(CancellationToken cancellationToken)
    {
        return await _dockerClient?.Images?.ListImagesAsync(new ImagesListParameters { }, cancellationToken);
    }

    public async Task<IList<ContainerListResponse>> ListContainersAsync(CancellationToken cancellationToken)
    {
        return await _dockerClient?.Containers?.ListContainersAsync(new ContainersListParameters { }, cancellationToken);
    }

    public async Task<DockerImageResDto> PullImageAsync(DockerImageReqDto dockerImageReqDto, CancellationToken cancellationToken)
    {
        string status = "";
        string imageHash = "";
        AuthConfig authConfig = null;

        var docker = _mapper.Map<AppDockerImage>(dockerImageReqDto);

        if (!String.IsNullOrEmpty(docker.ServerAddress) && !String.IsNullOrEmpty(docker.Username) && !String.IsNullOrEmpty(docker.Password))
        {
            authConfig = new AuthConfig
            {
                ServerAddress = docker.ServerAddress,
                Username = docker.Username,
                Password = docker.Password
            };
        }

        // Monitor pulling logs and extract Status and Hash
        Progress<JSONMessage> progress = new Progress<JSONMessage>();
        progress.ProgressChanged += (sender, message) =>
            {
                status = message?.Status;
                Console.WriteLine(status);
                if (!String.IsNullOrEmpty(status) && status.Contains("Digest:"))
                    imageHash = status.Split(" ")[1];
            };

        // Create an image by pulling from Docker Registry
        // Anonymously download an image by passing null for AuthConfig object
        await _dockerClient.Images.CreateImageAsync(new ImagesCreateParameters
        {
            FromImage = docker.RepositoryName,
            Tag = docker.Tag
        }, authConfig, progress);

        return new DockerImageResDto
        {
            Status = status,
            Hash = imageHash
        };
    }

    public async Task<DockerContainerStartResDto> ContainerStartAsync(DockerContainerReqDto dockerContainerReqDto, CancellationToken cancellationToken, ContainerListResponse c)
    {
        var docker = _mapper.Map<AppDockerContainer>(dockerContainerReqDto);

        var exposedPorts = new Dictionary<String, EmptyStruct>();
        var volumes = new Dictionary<String, EmptyStruct>();
        var hostConfig = new HostConfig();
        var socketDestination = "/var/run/docker.sock";
        var bind = new List<string>();
        var imageName = docker.RepositoryName + ":" + docker.Tag;
        var privatePort = "80";
        var publicPort = "8080";

        bind.Add(socketDestination + ":" + socketDestination);

        // Ports will be hardcoded here for demo, as there exists port is already allocated error
        if (imageName.Contains("postgres"))
            publicPort = privatePort ="5432";

        if (imageName.Contains("pgadmin4"))
            publicPort = "5050";

        if (imageName.Contains("redis"))
            publicPort = privatePort = "6379";

        if (imageName.Contains("webapi"))
            publicPort = "8001";

        if (imageName.Contains("webapi"))
            publicPort = "8001";

        if (imageName.Contains("webapp"))
            publicPort = "4200";

        if (imageName.Contains("dockerapi"))
        {
            // It'll fail to bind because in use. Leaving it empty.
            //publicPort = "9000";
            hostConfig = new HostConfig()
            {
                Binds = bind,
            };
        }
        else
        {
            hostConfig = new HostConfig()
            {
                PortBindings = new Dictionary<string, IList<PortBinding>> {
                    {
                        privatePort, new List<PortBinding> {
                            new PortBinding {
                                HostPort = publicPort
                            }
                        }
                    }
                }
            };
        }

        exposedPorts.Add(privatePort, new EmptyStruct());

        // Create container from image
        var createContainer = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters()
        {
            Image = imageName,
            ExposedPorts = exposedPorts,
            // Volumes = volumes,
            HostConfig = hostConfig
        });

        // Start created container
        var startStatus = await _dockerClient.Containers.StartContainerAsync(createContainer.ID, new ContainerStartParameters(), cancellationToken);

        return new DockerContainerStartResDto
        {
            Status = startStatus,
            CreateContainer = createContainer
        };
    }

    public async Task<DockerContainerStopResDto> ContainerStopAsync(DockerContainerReqDto dockerContainerReqDto, IList<ContainerListResponse> runningContainers, CancellationToken cancellationToken)
    {
        var stopDict = new Dictionary<string, bool>();

        var docker = _mapper.Map<AppDockerContainer>(dockerContainerReqDto);

        // Extract running containers spun up from the same Docker Image passed in request
        foreach (var c in runningContainers)
            if (c.Image == docker.RepositoryName + ":" + docker.Tag)
                stopDict.Add(c.ID, false);

        // Stop all discovered containers spun up from the same Docker Image
        foreach (KeyValuePair<string, bool> item in stopDict)
        {
            var stopped = await _dockerClient.Containers.StopContainerAsync(item.Key, new ContainerStopParameters { WaitBeforeKillSeconds = 30 }, cancellationToken);
            if (stopped)
                stopDict[item.Key] = true;
        }

        return new DockerContainerStopResDto
        {
            StopContainers = stopDict
        };
    }

    public DockerContainerStatsResDto ContainerStats(DockerContainerReqDto dockerContainerReqDto, IList<ContainerListResponse> runningContainers, CancellationToken cancellationToken)
    {
        List<ContainerListResponse> containerStats = new List<ContainerListResponse>();

        var docker = _mapper.Map<AppDockerContainer>(dockerContainerReqDto);

        foreach (var c in runningContainers)
            if (c.Image == docker.RepositoryName + ":" + docker.Tag)
                containerStats.Add(c);

        return new DockerContainerStatsResDto
        {
            ContainerStats = containerStats
        };
    }

}