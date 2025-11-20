using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.Auth;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.AuthValidators;

public class SignUpModelValidator : AbstractValidator<SignUpModel>
{
    public SignUpModelValidator()
    {
        RuleFor(x => x.FirstName)
            .MinimumLength(2).WithMessage(UserErrors.FirstNameRequired.Message);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(UserErrors.LastNameRequired.Message)
            .MinimumLength(2).WithMessage(UserErrors.LastNameRequired.Message);

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage(UserErrors.EmailInvalid.Message);

        RuleFor(x => x.UserName)
            .MinimumLength(3).WithMessage(UserErrors.UserNameInvalidLength.Message)
            .MaximumLength(20).WithMessage(UserErrors.UserNameInvalidLength.Message);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(UserErrors.PasswordRequired.Message);
    }
}
