namespace webapi.DTOs.UserService;

public class UpdateReqDto
{
    public UpdateStringField Username { get; set; }
    public UpdateStringField GivenName { get; set; }
    public UpdateStringField FamilyName { get; set; }
    public UpdateStringField Email { get; set; }
    public UpdateStringField Password { get; set; }
}