namespace webapi.DTOs.Docker

{
    public class DockerContainerDetailResDto
    {
        public Guid Id { get; set; }
        public string RepositoryName { get; set; }
        public string Tag { get; set; }
        public string Hash { get; set; }
        public string ExposedPort { get; set; }
        public string Volume { get; set; }
        public string SocketBind { get; set; }
        public string ContainerPort { get; set; }
        public string HostPort { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid UserId { get; set; }
    }
}