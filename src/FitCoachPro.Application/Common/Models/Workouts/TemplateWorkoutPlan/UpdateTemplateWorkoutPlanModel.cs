using FitCoachPro.Application.Common.Models.Workouts.TemplateWorkoutItem;

namespace FitCoachPro.Application.Common.Models.Workouts.TemplateWorkoutPlan;

public record UpdateTemplateWorkoutPlanModel(string TemplateName, IEnumerable<UpdateTemplateWorkoutItemModel> TemplateWorkoutItems);
