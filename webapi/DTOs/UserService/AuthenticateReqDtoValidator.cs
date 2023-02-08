namespace webapi.DTOs.UserService;

public class AuthenticateReqDtoValidator : AbstractValidator<AuthenticateReqDto>
{
    public AuthenticateReqDtoValidator(IStringLocalizer<Translation> l)
    {
        RuleFor(x => x.Username).Username(l);
        RuleFor(x => x.Password).Password(l);
    }
}