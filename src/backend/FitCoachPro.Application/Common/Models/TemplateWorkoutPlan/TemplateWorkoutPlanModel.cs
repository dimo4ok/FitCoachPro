using FitCoachPro.Application.Common.Models.TemplateWorkoutItem;

namespace FitCoachPro.Application.Common.Models.TemplateWorkoutPlan;

public record TemplateWorkoutPlanModel(Guid Id, string TemplateName, DateTime CreatedAt, DateTime? UpdatedAt, IEnumerable<TemplateWorkoutItemModel> TemplateWorkoutItems);
