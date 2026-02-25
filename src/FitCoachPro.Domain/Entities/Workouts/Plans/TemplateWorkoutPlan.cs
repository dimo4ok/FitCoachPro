using FitCoachPro.Domain.Entities.Users;
using FitCoachPro.Domain.Entities.Workouts.Items;

namespace FitCoachPro.Domain.Entities.Workouts.Plans;

public class TemplateWorkoutPlan : BaseWorkoutPlan
{
    public string TemplateName { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Guid CoachId { get; set; }
    public Coach Coach { get; set; } = null!;

    public ICollection<TemplateWorkoutItem> TemplateWorkoutItems { get; set; } = new List<TemplateWorkoutItem>();
}
