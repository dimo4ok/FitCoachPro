using FitCoachPro.Application.Common.Models.Workouts.WorkoutPlan;

namespace FitCoachPro.Application.Commands.WorkoutPlans.UpdateWorkoutPlan
{
    public record UpdateWorkoutPlanCommand(Guid WorkoutPlanId, UpdateWorkoutPlanModel Model);
}
