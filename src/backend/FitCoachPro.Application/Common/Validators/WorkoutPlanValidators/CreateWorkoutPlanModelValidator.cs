using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Models.WorkoutPlan;
using FitCoachPro.Application.Common.Validators.WorkoutItemValidators;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.WorkoutPlanValidators;

public class CreateWorkoutPlanModelValidator : AbstractValidator<CreateWorkoutPlanModel>
{
    public CreateWorkoutPlanModelValidator()
    {
        RuleFor(x => x.WorkoutDate)
            .NotEmpty()
                .WithErrorCode(WorkoutPlanErrors.WorkoutDataRequired.Code)
                .WithMessage(WorkoutPlanErrors.WorkoutDataRequired.Message)
            .Must(date => date.Date >= DateTime.UtcNow.Date)
                .WithErrorCode(WorkoutPlanErrors.DateCannotBeInPast.Code)
                .WithMessage(WorkoutPlanErrors.DateCannotBeInPast.Message);

        RuleFor(x => x.ClientId)
            .NotEmpty()
                .WithErrorCode(WorkoutPlanErrors.EmptyClientId.Code)
                .WithMessage(WorkoutPlanErrors.EmptyClientId.Message);

        RuleFor(x => x.WorkoutItems)
            .NotEmpty()
                .WithErrorCode(WorkoutPlanErrors.NotEnoughItems.Code)
                .WithMessage(WorkoutPlanErrors.NotEnoughItems.Message)
            .Must(items => items.Count() <= 10)
                .WithErrorCode(WorkoutPlanErrors.TooManyItems.Code)
                .WithMessage(WorkoutPlanErrors.TooManyItems.Message);

        RuleFor(x => x.WorkoutItems)
            .Must(items => items.Select(i => i.ExerciseId).Distinct().Count() == items.Count())
                .WithErrorCode(WorkoutPlanErrors.DuplicateExerciseId.Code)
                .WithMessage(WorkoutPlanErrors.DuplicateExerciseId.Message);

        RuleForEach(x => x.WorkoutItems).SetValidator(new CreateWorkoutItemModelValidator());
    }
}
