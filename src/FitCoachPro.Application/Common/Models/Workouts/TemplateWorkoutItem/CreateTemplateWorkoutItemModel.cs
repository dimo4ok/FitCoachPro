namespace FitCoachPro.Application.Common.Models.Workouts.TemplateWorkoutItem;

public record CreateTemplateWorkoutItemModel(int? Reps, int? Sets, string Description, Guid ExerciseId);