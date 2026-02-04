using FitCoachPro.Domain.Entities.Workouts.Plans;

namespace FitCoachPro.Domain.Entities.Workouts.Items;

public class TemplateWorkoutItem : BaseWorkoutItem
{
    public Guid TemplateWorkoutPlanId { get; set; }
    public TemplateWorkoutPlan TemplateWorkoutPlan { get; set; } = null!;
}
