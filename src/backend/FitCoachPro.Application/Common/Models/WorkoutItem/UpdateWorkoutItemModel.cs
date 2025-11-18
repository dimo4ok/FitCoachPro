namespace FitCoachPro.Application.Common.Models.WorkoutItem;

public record UpdateWorkoutItemModel(Guid? Id, string Description, Guid ExerciseId);
