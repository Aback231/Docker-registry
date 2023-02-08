namespace webapi.DTOs.Docker

{
    public class DockerImageUpdateReqDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Tag { get; set; }
        public string Hash { get; set; }
        public string CurrentVersion { get; set; }
        public string NewestVersion { get; set; }
        public bool NewestVersionPulled { get; set; }
    }
}