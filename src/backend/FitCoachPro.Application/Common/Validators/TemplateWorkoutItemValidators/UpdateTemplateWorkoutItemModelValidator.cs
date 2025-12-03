
using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.TemplateWorkoutItem;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.TemplateWorkoutItemValidators;

public class UpdateTemplateWorkoutItemModelValidator : AbstractValidator<UpdateTemplateWorkoutItemModel>
{
    public UpdateTemplateWorkoutItemModelValidator()
    {
        RuleFor(x => x.Reps)
         .InclusiveBetween(0, 100)
         .When(x => x.Reps.HasValue)
         .WithErrorCode(TemplateWorkoutItemErrors.RepsInvalid.Code)
         .WithMessage(TemplateWorkoutItemErrors.RepsInvalid.Message);

        RuleFor(x => x.Sets)
            .InclusiveBetween(0, 50)
            .When(x => x.Sets.HasValue)
            .WithErrorCode(TemplateWorkoutItemErrors.SetsInvalid.Code)
            .WithMessage(TemplateWorkoutItemErrors.SetsInvalid.Message);

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
