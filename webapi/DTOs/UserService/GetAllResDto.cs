namespace webapi.DTOs.UserService;

public class GetAllResDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public RoleResDto Role { get; set; }
}