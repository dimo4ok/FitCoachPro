using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Common.Errors;

public class WorkoutItemErrors
{
 public static Error InvalidWorkoutItemId => new Error("WorkoutItem.Invalid","WorkoutItem with Id is invalid for this plan.");
}

