namespace webapi.DTOs.Docker

{
    public class DockerImageDetailResDto
    {
        public Guid Id { get; set; }
        public string RepositoryName { get; set; }
        public string Tag { get; set; }
        public string Hash { get; set; }
        public string CurrentVersion { get; set; }
        public string NewestVersion { get; set; }
        public bool NewestVersionPulled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid UserId { get; set; }
    }
}