using FitCoachPro.Application.Common.Models.TemplateWorkoutItem;

namespace FitCoachPro.Application.Common.Models.TemplateWorkoutPlan;

public record CreateTemplateWorkoutPlanModel(string TemplateName, IEnumerable<CreateTemplateWorkoutItemModel> TemplateWorkoutItems);