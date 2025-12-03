using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.WorkoutItem;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.WorkoutItemValidators;

public class UpdateWorkoutItemModelValidator: AbstractValidator<UpdateWorkoutItemModel>
{
    public UpdateWorkoutItemModelValidator()
    {
        RuleFor(x => x.Reps)
           .InclusiveBetween(0, 100)
           .When(x => x.Reps.HasValue)
           .WithErrorCode(WorkoutItemErrors.RepsInvalid.Code)
           .WithMessage(WorkoutItemErrors.RepsInvalid.Message);

        RuleFor(x => x.Sets)
            .InclusiveBetween(0, 50)
            .When(x => x.Sets.HasValue)
            .WithErrorCode(WorkoutItemErrors.SetsInvalid.Code)
            .WithMessage(WorkoutItemErrors.SetsInvalid.Message);

        RuleFor(x => x.Description)
            .Length(3, 200)
            .WithErrorCode(WorkoutItemErrors.DescriptionInvalidLength.Code)
            .WithMessage(WorkoutItemErrors.DescriptionInvalidLength.Message);

        RuleFor(x => x.ExerciseId)
            .NotEmpty()
                .WithErrorCode(WorkoutItemErrors.ExerciseIdRequired.Code)
                .WithMessage(WorkoutItemErrors.ExerciseIdRequired.Message);
    }
}