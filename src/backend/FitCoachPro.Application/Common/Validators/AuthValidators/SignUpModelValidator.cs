using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.Auth;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.AuthValidators;

public class SignUpModelValidator : AbstractValidator<SignUpModel>
{
    public SignUpModelValidator()
    {
        RuleFor(x => x.FirstName)
            .MinimumLength(2)
                .WithErrorCode(UserErrors.FirstNameRequired.Code)
                .WithMessage(UserErrors.FirstNameRequired.Message);

        RuleFor(x => x.LastName)
            .NotEmpty()
                .WithErrorCode(UserErrors.LastNameRequired.Code)
                .WithMessage(UserErrors.LastNameRequired.Message)
            .MinimumLength(2)
                .WithErrorCode(UserErrors.LastNameRequired.Code)
                .WithMessage(UserErrors.LastNameRequired.Message);

        RuleFor(x => x.Email)
            .EmailAddress()
                .WithErrorCode(UserErrors.EmailInvalid.Code)
                .WithMessage(UserErrors.EmailInvalid.Message);

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
