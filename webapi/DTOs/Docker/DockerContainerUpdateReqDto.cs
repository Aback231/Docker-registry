namespace webapi.DTOs.Docker

{
    public class DockerContainerUpdateReqDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Tag { get; set; }
        public string Hash { get; set; }
    }
}