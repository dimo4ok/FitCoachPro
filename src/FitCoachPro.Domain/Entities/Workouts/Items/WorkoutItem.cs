using FitCoachPro.Domain.Entities.Workouts.Plans;

namespace FitCoachPro.Domain.Entities.Workouts.Items;

public class WorkoutItem : BaseWorkoutItem
{
    public Guid WorkoutPlanId { get; set; }
    public WorkoutPlan WorkoutPlan { get; set; } = null!;
}
