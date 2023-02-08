namespace webapi.DTOs.UserService;

public class GetDetailsResDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public string Email { get; set; }
    public RoleResDto Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }
}