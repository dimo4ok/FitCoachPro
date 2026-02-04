using FitCoachPro.Application.Common.Models.Workouts.Exercise;

namespace FitCoachPro.Application.Common.Models.Workouts.WorkoutItem;

public record WorkoutItemModel (Guid Id, int? Reps, int? Sets, string Description, Guid ExerciseId, ExerciseModel Exercise);
