using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Common.Errors;

public class ExerciseErrors
{
    public static Error NotFound => new("Exercise.NotFound", "Exercise not found.");
    public static Error InvalidExerciseId => new("Exercise.Invalid", "ExerciseId does not exist.");
}
