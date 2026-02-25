using FitCoachPro.Application.Common.Models.Workouts.TemplateWorkoutPlan;

namespace FitCoachPro.Application.Commands.TemplateWorkoutPlans.UpdateTemplateWorkoutPlan;

public record UpdateTemplateCommand(Guid Id, UpdateTemplateWorkoutPlanModel Model);
