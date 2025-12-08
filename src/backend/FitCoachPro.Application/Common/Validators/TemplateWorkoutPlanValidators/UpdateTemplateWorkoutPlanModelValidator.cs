using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.Workouts.TemplateWorkoutPlan;
using FitCoachPro.Application.Common.Validators.TemplateWorkoutItemValidators;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Domain.Entities.Workouts.Items;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.TemplateWorkoutPlanValidators;

public class UpdateTemplateWorkoutPlanModelValidator : AbstractValidator<UpdateTemplateWorkoutPlanModel>
{
    public UpdateTemplateWorkoutPlanModelValidator()
    {
        RuleFor(x => x.TemplateName)
           .NotEmpty()
           .Length(3, 50);

        RuleFor(x => x.TemplateWorkoutItems)
          .NotEmpty()
          .Must(items => items.Any() && items.Count() <= 10)
              .WithMessage(ValidationErrors.CollectionSizeInvalid(nameof(TemplateWorkoutItem)).Message);

        RuleFor(x => x.TemplateWorkoutItems)
            .Must(items => items.Where(i => i.Id.HasValue).Select(i => i.Id!.Value).Distinct().Count() == items.Count(i => i.Id.HasValue))
                .WithMessage(ValidationErrors.DuplicateId(nameof(TemplateWorkoutItem)).Message);

        RuleFor(x => x.TemplateWorkoutItems)
            .Must(items => items.Select(i => i.ExerciseId).Distinct().Count() == items.Count())
                .WithMessage(ValidationErrors.CollectionSizeInvalid(nameof(Exercise)).Message);

        RuleForEach(x => x.TemplateWorkoutItems).SetValidator(new UpdateTemplateWorkoutItemModelValidator());
    }
}
