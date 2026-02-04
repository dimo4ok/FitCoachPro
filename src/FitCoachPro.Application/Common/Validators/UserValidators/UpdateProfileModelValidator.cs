using FitCoachPro.Application.Common.Models.Users;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.UserValidators;

public class UpdateProfileModelValidator : AbstractValidator<UpdateProfileModel>
{
    public UpdateProfileModelValidator()
    {
        RuleFor(x => x.FirstName)
           .MinimumLength(2);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MinimumLength(2);

        RuleFor(x => x.Email)
            .EmailAddress();
    }
}
