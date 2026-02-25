namespace FitCoachPro.Application.Common.Models.Workouts.WorkoutItem;

public record CreateWorkoutItemModel(int? Reps, int? Sets, string Description, Guid ExerciseId);
