namespace dockerapi.DTOs

{
    public class DockerContainerStartResDto
    {
        public bool Status { get; set; }
        public CreateContainerResponse CreateContainer { get; set; }
    }
}