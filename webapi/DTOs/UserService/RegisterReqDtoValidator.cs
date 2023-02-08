namespace webapi.DTOs.UserService;

public class RegisterReqDtoValidator : AbstractValidator<RegisterReqDto>
{
    public RegisterReqDtoValidator(IStringLocalizer<Translation> l)
    {
        RuleFor(x => x.Username).Username(l);
        // RuleFor(x => x.GivenName).GivenName(l);
        // RuleFor(x => x.FamilyName).FamilyName(l);
        // RuleFor(x => x.Email).Email(l);
        RuleFor(x => x.Password).Password(l);
    }
}