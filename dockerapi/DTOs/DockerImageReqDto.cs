namespace dockerapi.DTOs

{
    public class DockerImageReqDto
    {
        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string RepositoryName { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Tag { get; set; }
        public string ServerAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}