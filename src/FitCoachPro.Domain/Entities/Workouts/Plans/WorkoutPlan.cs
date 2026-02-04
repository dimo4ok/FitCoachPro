using FitCoachPro.Domain.Entities.Users;
using FitCoachPro.Domain.Entities.Workouts.Items;

namespace FitCoachPro.Domain.Entities.Workouts.Plans;

public class WorkoutPlan : BaseWorkoutPlan
{
    public DateTime WorkoutDate { get; set; }

    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public ICollection<WorkoutItem> WorkoutItems { get; set; } = new List<WorkoutItem>();
}