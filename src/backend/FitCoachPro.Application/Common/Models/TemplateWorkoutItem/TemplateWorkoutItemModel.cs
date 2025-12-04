using FitCoachPro.Application.Common.Models.Exercise;

namespace FitCoachPro.Application.Common.Models.TemplateWorkoutItem;

public record class TemplateWorkoutItemModel(Guid Id, int? Reps, int? Sets, string Description, Guid ExercsieId, ExerciseModel Exercise);
