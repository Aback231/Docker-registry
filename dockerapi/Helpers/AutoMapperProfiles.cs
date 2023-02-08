namespace dockerapi.Helpers

{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<DockerImageReqDto, AppDockerImage>();
            CreateMap<DockerContainerReqDto, AppDockerContainer>();
        }
    }
}