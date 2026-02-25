using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.Workouts.WorkoutPlan;
using FitCoachPro.Application.Common.Validators.WorkoutItemValidators;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Domain.Entities.Workouts.Items;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.WorkoutPlanValidators;

public class UpdateWorkoutPlanModelValidator : AbstractValidator<UpdateWorkoutPlanModel>
{
    public UpdateWorkoutPlanModelValidator()
    {
        RuleFor(x => x.WorkoutDate)
            .NotEmpty()
            .Must(date => date.Date >= DateTime.UtcNow.Date)
                .WithMessage(ValidationErrors.DateCannotBeInPast.Message);

        RuleFor(x => x.WorkoutItems)
            .NotEmpty()
            .Must(items => items.Any() && items.Count() <= 10)
                .WithMessage(ValidationErrors.CollectionSizeInvalid(nameof(WorkoutItem)).Message);

        RuleFor(x => x.WorkoutItems)
            .Must(items => items.Where(i => i.Id.HasValue).Select(i => i.Id!.Value).Distinct().Count() == items.Count(i => i.Id.HasValue))
                .WithMessage(ValidationErrors.DuplicateId(nameof(WorkoutItem)).Message);

        RuleFor(x => x.WorkoutItems)
            .Must(items => items.Select(i => i.ExerciseId).Distinct().Count() == items.Count())
                .WithMessage(ValidationErrors.DuplicateId(nameof(Exercise)).Message);

        RuleForEach(x => x.WorkoutItems).SetValidator(new UpdateWorkoutItemModelValidator());
    }
}
