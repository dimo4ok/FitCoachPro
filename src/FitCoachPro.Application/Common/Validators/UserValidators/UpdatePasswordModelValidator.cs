using FitCoachPro.Application.Common.Models.Users;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.UserValidators;

public class UpdatePasswordModelValidator : AbstractValidator<UpdatePasswordModel>
{
    public UpdatePasswordModelValidator()
    {
        RuleFor(x => x.Password)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty();

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.NewPassword);
    }
}
