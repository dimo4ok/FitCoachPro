using FitCoachPro.Application.Common.Models.Workouts.Exercise;

namespace FitCoachPro.Application.Common.Models.Workouts.TemplateWorkoutItem;

public record class TemplateWorkoutItemModel(Guid Id, int? Reps, int? Sets, string Description, Guid ExercsieId, ExerciseModel Exercise);
