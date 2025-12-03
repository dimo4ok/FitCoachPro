namespace FitCoachPro.Application.Common.Models.WorkoutItem;

public record UpdateWorkoutItemModel(Guid? Id, int? Reps, int? Sets, string Description, Guid ExerciseId);
