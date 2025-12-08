using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Workouts.Plans;

namespace FitCoachPro.Domain.Entities.Users;

public class Coach : UserProfile
{
    public ClientAcceptanceStatus AcceptanceStatus { get; set; } = ClientAcceptanceStatus.Accepting;

    public ICollection<Client> Clients { get; set; } = new List<Client>();

    public ICollection<TemplateWorkoutPlan> TemplateWorkoutPlans { get; set; } = new List<TemplateWorkoutPlan>();
}
