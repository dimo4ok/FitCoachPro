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
           .When(x => x.Reps.HasValue);

        RuleFor(x => x.Sets)
            .InclusiveBetween(0, 50)
            .When(x => x.Sets.HasValue);

        RuleFor(x => x.Description)
            .Length(3, 200);

        RuleFor(x => x.ExerciseId)
            .NotEmpty();
    }
}