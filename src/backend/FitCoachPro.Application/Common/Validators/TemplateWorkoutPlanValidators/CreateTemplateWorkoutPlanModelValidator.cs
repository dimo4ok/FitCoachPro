using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.TemplateWorkoutPlan;
using FitCoachPro.Application.Common.Validators.TemplateWorkoutItemValidators;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.TemplateWorkoutPlanValidators;

public class CreateTemplateWorkoutPlanModelValidator : AbstractValidator<CreateTemplateWorkoutPlanModel>
{
    public CreateTemplateWorkoutPlanModelValidator()
    {
        RuleFor(x => x.TemplateName)
            .NotEmpty()
                .WithErrorCode(TemplateWorkoutPlanErrors.TemplateNameRequired.Code)
                .WithMessage(TemplateWorkoutPlanErrors.TemplateNameRequired.Message)
            .Length(3, 50)
                .WithErrorCode(TemplateWorkoutPlanErrors.InvalidTemplateNameLength.Code)
                .WithMessage(TemplateWorkoutPlanErrors.InvalidTemplateNameLength.Message);

        RuleFor(x => x.TemplateWorkoutItems)
            .NotEmpty()
                .WithErrorCode(TemplateWorkoutPlanErrors.NotEnoughItems.Code)
                .WithMessage(TemplateWorkoutPlanErrors.NotEnoughItems.Message)
            .Must(items => items.Count() <= 10)
                .WithErrorCode(TemplateWorkoutPlanErrors.TooManyItems.Code)
                .WithMessage(TemplateWorkoutPlanErrors.TooManyItems.Message);

        RuleFor(x => x.TemplateWorkoutItems)
            .Must(items => items.Select(i => i.ExerciseId).Distinct().Count() == items.Count())
                .WithErrorCode(TemplateWorkoutPlanErrors.DuplicateExerciseId.Code)
                .WithMessage(TemplateWorkoutPlanErrors.DuplicateExerciseId.Message);

        RuleForEach(x => x.TemplateWorkoutItems).SetValidator(new CreateTemplateWorkoutItemModelValidator());
    }
}
