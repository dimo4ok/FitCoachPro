using FitCoachPro.Application.Common.Models.Exercise;

namespace FitCoachPro.Application.Common.Models.WorkoutItem;

public record WorkoutItemModel (Guid Id, string Description, Guid ExerciseId, ExerciseNestedModel Exercise);
