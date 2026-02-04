using FitCoachPro.Application.Common.Models.Workouts.TemplateWorkoutItem;

namespace FitCoachPro.Application.Common.Models.Workouts.TemplateWorkoutPlan;

public record CreateTemplateWorkoutPlanModel(string TemplateName, IEnumerable<CreateTemplateWorkoutItemModel> TemplateWorkoutItems);