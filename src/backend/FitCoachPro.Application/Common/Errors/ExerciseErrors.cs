using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Common.Errors;

public class ExerciseErrors
{
    public static Error Forbidden => new("Exercise.Forbidden", "You do not have access to exercsie.");
    public static Error AlreadyExists => new("Exercise.AlreadyExists", "An exercsie with this name already exists.");
    public static Error NotFound => new("Exercise.NotFound", "Exercsie not found.");
    public static Error InvalidExerciseId => new("Exercise.Invalid", "ExerciseId does not exist.");
    public static Error UsedInActiveWorkoutPlan => 
        new("Exercise.UsedInActiveWorkoutPlan", "This exercise is used in an active workout plan and cannot be deleted.");
    public static Error ConcurrencyConflict =>
        new("Exercise.ConcurrencyConflict", "The exercise was modified by another user. Please refresh and try again.");

    public static Error NameEmpty => new("Exercise.NameEmpty", "Exercise name must not be empty.");
    public static Error GifUrlEmpty => new("Exercise.GifUrlEmpty", "GifUrl must not be empty.");
    public static Error NameInvalid => new("Exercise.NameInvalid", "Exercise name must be between 1 and 50 characters.");
    public static Error GifUrlInvalid => new("Exercise.GifUrlInvalid", "GifUrl must be between 1 and 100 characters.");
    public static Error RowVersionMissing => new("Exercise.RowVersionMissing", "RowVersion must be provided for concurrency check.");
}
