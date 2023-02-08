namespace webapi.DTOs.UserService;

public class AuthenticateResDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
}