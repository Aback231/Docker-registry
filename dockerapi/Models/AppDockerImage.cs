namespace dockerapi.Models

{
    public class AppDockerImage
    {
        public string RepositoryName { get; set; }
        public string Tag { get; set; }
        public string ServerAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}