namespace webapi.Helpers;

public class AppSettings
{
    public string Secret { get; set; }
    public int MaxLoginFailedCount { get; set; }
    public int LoginFailedWaitingTime { get; set; }
    public string AdminUsername { get; set; }
    public string AdminEmail { get; set; }
    public string AdminPassword { get; set; }
    public string ApiVersion { get; set; }
}