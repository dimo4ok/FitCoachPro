using FitCoachPro.Application.Common.Models.Workouts.TemplateWorkoutPlan;
using FitCoachPro.Domain.Entities.Workouts.Plans;

namespace FitCoachPro.Application.Common.Extensions.WorkoutExtensions;

public static class TemplateWorkoutPlanExtensions
{
    public static TemplateWorkoutPlanModel ToModel(this TemplateWorkoutPlan templatePlan) =>
        new(
            templatePlan.Id,
            templatePlan.TemplateName,
            templatePlan.CreatedAt,
            templatePlan.UpdatedAt,
            templatePlan.TemplateWorkoutItems
                .Select(x => x.ToModel())
                .ToList()
                .AsReadOnly());

    public static TemplateWorkoutPlan ToEntity(this CreateTemplateWorkoutPlanModel templatePlan, Guid coachId) =>
        new()
        {
            TemplateName = templatePlan.TemplateName,
            CreatedAt = DateTime.UtcNow,
            CoachId = coachId,
            TemplateWorkoutItems = templatePlan.TemplateWorkoutItems
                .Select(x => x.ToEntity())
                .ToList()
                .AsReadOnly()
        };
}
