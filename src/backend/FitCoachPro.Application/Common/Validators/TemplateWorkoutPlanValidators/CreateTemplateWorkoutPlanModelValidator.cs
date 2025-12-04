using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.TemplateWorkoutPlan;
using FitCoachPro.Application.Common.Validators.TemplateWorkoutItemValidators;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Domain.Entities.Workouts.Items;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.TemplateWorkoutPlanValidators;

public class CreateTemplateWorkoutPlanModelValidator : AbstractValidator<CreateTemplateWorkoutPlanModel>
{
    public CreateTemplateWorkoutPlanModelValidator()
    {
        RuleFor(x => x.TemplateName)
            .NotEmpty()
            .Length(3, 50);

        RuleFor(x => x.TemplateWorkoutItems)
            .NotEmpty()
            .Must(items => items.Any() && items.Count() <= 10)
                .WithMessage(ValidationErrors.CollectionSizeInvalid(nameof(TemplateWorkoutItem)).Message);

        RuleFor(x => x.TemplateWorkoutItems)
            .Must(items => items.Select(i => i.ExerciseId).Distinct().Count() == items.Count())
                .WithMessage(ValidationErrors.CollectionSizeInvalid(nameof(Exercise)).Message);

        RuleForEach(x => x.TemplateWorkoutItems).SetValidator(new CreateTemplateWorkoutItemModelValidator());
    }
}
