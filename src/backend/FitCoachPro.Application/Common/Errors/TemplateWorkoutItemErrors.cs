using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Common.Errors;

public static class TemplateWorkoutItemErrors
{
    public static Error InvalidTempalteWorkoutItemId => new("TempalteWorkoutItem.Invalid", "TempalteWorkoutItem with Id is invalid for this plan.");

    public static Error IdCannotBeEmpty => new("TempalteWorkoutItem.IdCannotBeEmpty", "TempalteWorkoutItem ID cannot be an empty.");
    public static Error DescriptionRequired => new("TempalteWorkoutItem.DescriptionRequired", "Description must be provided and be at least 3 characters long.");
    public static Error DescriptionInvalidLength => new("TempalteWorkoutItem.DescriptionInvalidLength", "Description must be between 3 and 200 characters long.");

    public static Error ExerciseIdRequired => new("TempalteWorkoutItem.ExerciseIdRequired", "Exercise ID must be provided.");
}
