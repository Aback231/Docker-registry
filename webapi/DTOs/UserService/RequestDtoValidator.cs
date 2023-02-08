namespace webapi.DTOs.UserService;

public class UpdateAsyncReqDtoValidator : AbstractValidator<UpdateReqDto>
{
    public UpdateAsyncReqDtoValidator(IStringLocalizer<Translation> l)
    {
        RuleFor(x => x.Username.NewValue).Username(l).When(y => y.Username != null);
        // RuleFor(x => x.GivenName.NewValue).GivenName(l).When(y => y.GivenName != null);
        // RuleFor(x => x.FamilyName.NewValue).FamilyName(l).When(y => y.FamilyName != null);
        // RuleFor(x => x.Email.NewValue).Email(l).When(y => y.Email != null);
        RuleFor(x => x.Password.NewValue).Password(l).When(y => y.Password != null);
    }
}