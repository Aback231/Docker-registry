namespace webapi.DTOs;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> Username<T>(this IRuleBuilder<T, string> ruleBuilderOptions,
        IStringLocalizer l)
    {
        return ruleBuilderOptions
            .NotEmpty().WithMessage(l["Username cannot be empty."])
            .MinimumLength(3)
            .WithMessage(string.Format(l["Username must be at least {0} characters long."], 3))
            .MaximumLength(20)
            .WithMessage(string.Format(l["Username cannot be longer than {0} characters."], 20));
    }

    public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilderOptions,
        IStringLocalizer l)
    {
        return ruleBuilderOptions
            .NotEmpty().WithMessage(l["Password cannot be empty."])
            .MinimumLength(6)
            .WithMessage(string.Format(l["Password must be at least {0} characters long."], 6))
            .Matches("[A-Z]").WithMessage(l["Password must contain at least one uppercase letter."])
            .Matches("[a-z]").WithMessage(l["Password must contain at least one lowercase letter."])
            .Matches("[0-9]").WithMessage(l["Password must contain at least one digit."]);
    }

    // public static IRuleBuilderOptions<T, string> GivenName<T>(this IRuleBuilder<T, string> ruleBuilderOptions,
    //     IStringLocalizer l)
    // {
    //     return ruleBuilderOptions
    //         .NotEmpty().WithMessage(l["Given name cannot be empty."])
    //         .MinimumLength(2)
    //         .WithMessage(string.Format(l["Given name must be at least {0} characters long."], 2))
    //         .MaximumLength(30)
    //         .WithMessage(string.Format(l["Given name cannot be longer than {0} characters."], 30));
    // }

    // public static IRuleBuilderOptions<T, string> FamilyName<T>(this IRuleBuilder<T, string> ruleBuilderOptions,
    //     IStringLocalizer l)
    // {
    //     return ruleBuilderOptions
    //         .NotEmpty().WithMessage(l["Family name cannot be empty."])
    //         .MinimumLength(2)
    //         .WithMessage(string.Format(l["Family name must be at least {0} characters long."], 2))
    //         .MaximumLength(30)
    //         .WithMessage(string.Format(l["Family name cannot be longer than {0} characters."], 30));
    // }

    public static IRuleBuilderOptions<T, string> Email<T>(this IRuleBuilder<T, string> ruleBuilderOptions,
        IStringLocalizer l)
    {
        return ruleBuilderOptions.EmailAddress().WithMessage(l["Invalid email address."]);
    }
}