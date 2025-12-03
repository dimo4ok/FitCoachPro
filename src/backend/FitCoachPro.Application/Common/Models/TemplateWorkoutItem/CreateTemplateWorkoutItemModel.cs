namespace FitCoachPro.Application.Common.Models.TemplateWorkoutItem;

public record CreateTemplateWorkoutItemModel(int? Reps, int? Sets, string Description, Guid ExerciseId);