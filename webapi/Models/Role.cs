namespace webapi.Models;

public class Role
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedById { get; set; }
    public virtual User CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedById { get; set; }
    public virtual User UpdatedBy { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public virtual ICollection<User> Users { get; set; }
}