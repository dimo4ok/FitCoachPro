using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.WorkoutItem;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.WorkoutItemValidators;

public class CreateWorkoutItemModelValidator : AbstractValidator<CreateWorkoutItemModel>
{
    public CreateWorkoutItemModelValidator()
    {
        RuleFor(x => x.Description)
           .Length(3, 200).WithMessage(WorkoutItemErrors.DescriptionInvalidLength.Message);

        RuleFor(x => x.ExerciseId)
            .NotEmpty().WithMessage(WorkoutItemErrors.ExerciseIdRequired.Message);
    }
}
