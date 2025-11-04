namespace FitCoachPro.Domain.Entities.Workouts.Items;

public abstract class BaseWorkoutItem
{
    public Guid Id { get; set; }
    public string Description { get; set; } = null!;

    public Guid ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;
}
