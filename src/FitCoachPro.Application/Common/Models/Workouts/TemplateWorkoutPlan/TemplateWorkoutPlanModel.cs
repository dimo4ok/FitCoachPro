using FitCoachPro.Application.Common.Models.Workouts.TemplateWorkoutItem;

namespace FitCoachPro.Application.Common.Models.Workouts.TemplateWorkoutPlan;

public record TemplateWorkoutPlanModel(Guid Id, string TemplateName, DateTime CreatedAt, DateTime? UpdatedAt, IEnumerable<TemplateWorkoutItemModel> TemplateWorkoutItems);
