
using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.TemplateWorkoutItem;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.TemplateWorkoutItemValidators;

public class UpdateTemplateWorkoutItemModelValidator : AbstractValidator<UpdateTemplateWorkoutItemModel>
{
    public UpdateTemplateWorkoutItemModelValidator()
    {
        RuleFor(x => x.Description)
           .Length(3, 200)
           .WithErrorCode(TemplateWorkoutItemErrors.DescriptionInvalidLength.Code)
           .WithMessage(TemplateWorkoutItemErrors.DescriptionInvalidLength.Message);

        RuleFor(x => x.ExerciseId)
            .NotEmpty()
                .WithErrorCode(TemplateWorkoutItemErrors.ExerciseIdRequired.Code)
                .WithMessage(TemplateWorkoutItemErrors.ExerciseIdRequired.Message);
    }
}
