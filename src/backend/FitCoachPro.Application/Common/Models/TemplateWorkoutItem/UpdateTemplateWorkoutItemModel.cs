namespace FitCoachPro.Application.Common.Models.TemplateWorkoutItem;

public record UpdateTemplateWorkoutItemModel(Guid? Id, int? Reps, int? Sets, string Description, Guid ExerciseId);