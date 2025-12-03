namespace FitCoachPro.Application.Common.Models.WorkoutItem;

public record CreateWorkoutItemModel(int? Reps, int? Sets, string Description, Guid ExerciseId);
