using FitCoachPro.Domain.Entities.Workouts.Plans;

namespace FitCoachPro.Domain.Entities.Users;

public class Coach : UserProfile
{
    public ICollection<Client> Clients { get; set; } = new List<Client>();

    public ICollection<TemplateWorkoutPlan> TemplateWorkoutPlans { get; set; } = new List<TemplateWorkoutPlan>();
}
