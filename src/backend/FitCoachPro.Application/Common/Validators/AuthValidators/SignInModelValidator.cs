using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.Auth;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.AuthValidators;

public class SignInModelValidator : AbstractValidator<SignInModel>
{
    public SignInModelValidator()
    {
        RuleFor(x => x.UserName)
            .MinimumLength(3)
                .WithErrorCode(UserErrors.UserNameInvalidLength.Code)
                .WithMessage(UserErrors.UserNameInvalidLength.Message)
            .MaximumLength(20)
                .WithErrorCode(UserErrors.UserNameInvalidLength.Code)
                .WithMessage(UserErrors.UserNameInvalidLength.Message);

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithErrorCode(UserErrors.PasswordRequired.Code)
                .WithMessage(UserErrors.PasswordRequired.Message);
    }
}
