using FitCoachPro.Domain.Entities.Workouts.Plans;

namespace FitCoachPro.Domain.Entities.Users;

public class Client : UserProfile
{
    public DateTime? SubscriptionExpiresAt { get; set; }

    public Guid? CoachId { get; set; }
    public Coach? Coach { get; set; }

    public ICollection<WorkoutPlan> WorkoutPlans { get; set; } = new List<WorkoutPlan>();
}
