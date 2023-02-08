namespace webapi.DTOs.Docker

{
    public class DockerContainerReqDto
    {
        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string RepositoryName { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Tag { get; set; }
        public string Hash { get; set; }
        public string ExposedPort { get; set; }
        public string Volume { get; set; }
        public string SocketBind { get; set; }
        public string ContainerPort { get; set; }
        public string HostPort { get; set; }
    }
}