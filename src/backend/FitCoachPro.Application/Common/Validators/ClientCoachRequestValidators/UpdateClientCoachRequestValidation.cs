using FitCoachPro.Application.Common.Models.Requests;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.ClientCoachRequestValidators;

public class UpdateClientCoachRequestValidation : AbstractValidator<UpdateClientCoachRequestModel>
{
    public UpdateClientCoachRequestValidation()
    {
        RuleFor(x => x.status)
            .NotEmpty();

        RuleFor(x => x.RowVersion)
            .NotEmpty();
    }
}
