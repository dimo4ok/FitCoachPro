using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.Auth;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.AuthValidators;

public class SignUpModelValidator : AbstractValidator<SignUpModel>
{
    public SignUpModelValidator()
    {
        RuleFor(x => x.FirstName)
            .MinimumLength(2);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MinimumLength(2);

        RuleFor(x => x.Email)
            .EmailAddress();

        RuleFor(x => x.UserName)
            .MinimumLength(3)
            .MaximumLength(20);

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
