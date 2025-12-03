using FitCoachPro.Application.Common.Models.Exercise;

namespace FitCoachPro.Application.Common.Models.WorkoutItem;

public record WorkoutItemModel (Guid Id, int? Reps, int? Sets, string Description, Guid ExerciseId, ExerciseNestedModel Exercise);
