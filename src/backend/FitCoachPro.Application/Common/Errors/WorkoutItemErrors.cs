using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Common.Errors;

public class WorkoutItemErrors
{
    public static Error InvalidWorkoutItemId => new("WorkoutItem.Invalid", "WorkoutItem with Id is invalid for this plan.");

    public static Error IdCannotBeEmpty => new("WorkoutItem.IdCannotBeEmpty", "WorkoutItem ID cannot be an empty.");
    public static Error DescriptionRequired => new("WorkoutItem.DescriptionRequired", "Description must be provided and be at least 3 characters long.");
    public static Error DescriptionInvalidLength => new("WorkoutItem.DescriptionInvalidLength", "Description must be between 3 and 200 characters long.");

    public static Error ExerciseIdRequired => new("WorkoutItem.ExerciseIdRequired", "Exercise ID must be provided.");
}

