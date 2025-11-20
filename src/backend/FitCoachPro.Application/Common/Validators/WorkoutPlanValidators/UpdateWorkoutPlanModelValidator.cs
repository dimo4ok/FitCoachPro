using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.WorkoutPlan;
using FitCoachPro.Application.Common.Validators.WorkoutItemValidators;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.WorkoutPlanValidators;

public class UpdateWorkoutPlanModelValidator : AbstractValidator<UpdateWorkoutPlanModel>
{
    public UpdateWorkoutPlanModelValidator()
    {
        RuleFor(x => x.WorkoutDate)
            .NotEmpty().WithMessage(WorkoutPlanErrors.WorkoutDataRequired.Message)
            .Must(date => date >= DateTime.UtcNow).WithMessage(WorkoutPlanErrors.DateCannotBeInPast.Message);

        RuleFor(x => x.ClientId)
               .NotEmpty().WithMessage(WorkoutPlanErrors.EmptyClientId.Message);

        RuleFor(x => x.WorkoutItems)
               .NotEmpty().WithMessage(WorkoutPlanErrors.NotEnoughItems.Message)
               .Must(items => items.Count() <= 10).WithMessage(WorkoutPlanErrors.TooManyItems.Message);

        RuleFor(x => x.WorkoutItems)
            .Must(items => items.Where(i => i.Id.HasValue).Select(i => i.Id!.Value).Distinct().Count() == items.Count(i => i.Id.HasValue))
            .WithMessage(WorkoutPlanErrors.DuplicateWorkoutItemId.Message);

        RuleFor(x => x.WorkoutItems)
            .Must(items => items.Select(i => i.ExerciseId).Distinct().Count() == items.Count())
            .WithMessage(WorkoutPlanErrors.DuplicateExerciseId.Message);

        RuleForEach(x => x.WorkoutItems).SetValidator(new UpdateWorkoutItemModelValidator());
    }
}
