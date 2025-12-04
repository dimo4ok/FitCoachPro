using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.WorkoutPlan;
using FitCoachPro.Application.Common.Validators.WorkoutItemValidators;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Domain.Entities.Workouts.Items;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.WorkoutPlanValidators;

public class CreateWorkoutPlanModelValidator : AbstractValidator<CreateWorkoutPlanModel>
{
    public CreateWorkoutPlanModelValidator()
    {
        RuleFor(x => x.WorkoutDate)
            .NotEmpty()
            .Must(date => date.Date >= DateTime.UtcNow.Date)
                .WithErrorCode(ValidationErrors.DateCannotBeInPast.Message);

        RuleFor(x => x.ClientId)
            .NotEmpty();

        RuleFor(x => x.WorkoutItems)
            .NotEmpty()
            .Must(items => items.Any() && items.Count() <=10)
                .WithMessage(ValidationErrors.CollectionSizeInvalid(nameof(WorkoutItem)).Message);

        RuleFor(x => x.WorkoutItems)
           .Must(items => items.Select(i => i.ExerciseId).Distinct().Count() == items.Count())
                .WithMessage(ValidationErrors.DuplicateId(nameof(Exercise)).Message);

        RuleForEach(x => x.WorkoutItems).SetValidator(new CreateWorkoutItemModelValidator());
    }
}
