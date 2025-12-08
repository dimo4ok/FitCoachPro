using FitCoachPro.Application.Common.Models.Requests;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.ClientCoachRequestValidators;

class DeleteClientCoachRequestValidation : AbstractValidator<DeleteClientCoachRequestModel>
{
    public DeleteClientCoachRequestValidation()
    {
        RuleFor(x => x.RowVersion)
            .NotEmpty();
    }
}
