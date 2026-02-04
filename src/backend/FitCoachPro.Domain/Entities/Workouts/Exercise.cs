namespace FitCoachPro.Domain.Entities.Workouts;

public class Exercise
{
    public Guid Id { get; set; }
    public string ExerciseName { get; set; } = null!;
    public string GifUrl { get; set; } = null!;

    public uint RowVersion { get; set; }
}
