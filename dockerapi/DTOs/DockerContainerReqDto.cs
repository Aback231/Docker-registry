namespace dockerapi.DTOs

{
    public class DockerContainerReqDto
    {
        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string RepositoryName { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Tag { get; set; }
    }
}