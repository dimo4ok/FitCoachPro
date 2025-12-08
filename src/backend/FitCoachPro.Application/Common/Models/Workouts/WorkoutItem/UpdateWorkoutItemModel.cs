namespace FitCoachPro.Application.Common.Models.Workouts.WorkoutItem;

public record UpdateWorkoutItemModel(Guid? Id, int? Reps, int? Sets, string Description, Guid ExerciseId);
