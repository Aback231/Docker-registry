namespace webapi.DTOs.UserService;

public class RegisterReqDto
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public string Email { get; set; }
}