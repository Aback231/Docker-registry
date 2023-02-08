namespace dockerapi.DTOs

{
    public class DockerContainerUpdateResDto
    {
        public bool Status { get; set; }
        public CreateContainerResponse CreateContainer { get; set; }
        public Dictionary<string, bool> StopContainers { get; set; }
    }
}