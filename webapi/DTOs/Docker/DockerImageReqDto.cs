namespace webapi.DTOs.Docker

{
    public class DockerImageReqDto
    {
        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string RepositoryName { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Tag { get; set; }
    }
}