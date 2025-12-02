using FitCoachPro.Application.Common.Models.TemplateWorkoutItem;

namespace FitCoachPro.Application.Common.Models.TemplateWorkoutPlan;

public record UpdateTemplateWorkoutPlanModel(string TemplateName, IEnumerable<UpdateTemplateWorkoutItemModel> TemplateWorkoutItems);
