namespace webapi.Models;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedById { get; set; }
    public virtual User CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedById { get; set; }
    public virtual User UpdatedBy { get; set; }
    public Guid? RoleId { get; set; }
    public virtual Role Role { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? LoginFailedAt { get; set; }
    public int LoginFailedCount { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public bool IsActive { get; set; }
    public virtual ICollection<User> CreatedUsers { get; set; }
    public virtual ICollection<User> UpdatedUsers { get; set; }
    public virtual ICollection<Role> CreatedRoles { get; set; }
    public virtual ICollection<Role> UpdatedRoles { get; set; }
    public virtual ICollection<DockerContainer> DockerContainers { get; set; }
    public virtual ICollection<DockerImage> DockerImages { get; set; }
}