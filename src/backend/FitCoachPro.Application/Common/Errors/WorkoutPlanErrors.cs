
using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Common.Errors;

public class WorkoutPlanErrors
{
    public static Error NotFound => new("WorkoutPlan.NotFound", "Workout plan not found.");
    public static Error Forbidden => new("WorkoutPlan.Forbidden", "You do not have access to workout plans for this client.");
    public static Error AlreadyExists => new("WorkoutPlan.AlreadyExists", "A workout plan for this date already exists.");

    public static Error WorkoutDataRequired => new("WorkoutPlan.WorkoutDataRequired", "Workout date must be provided");
    public static Error DateCannotBeInPast => new("WorkoutPlan.DateCannotBeInPast", "Workout date cannot be in the past.");
    public static Error EmptyClientId => new("WorkoutPlan.CleintId.MustBeFiled", "Workout client id must be field");

    public static Error DuplicateWorkoutItemId => new("WorkoutPlan.DuplicateId", "WorkoutItems collection cannot contain duplicate Ids.");
    public static Error NotEnoughItems => new("WorkoutPlan.NotEnoughItems", "Workout must include at least one exercise.");
    public static Error TooManyItems => new("WorkoutPlan.TooManyItems", "Workout cannot include more than 10 exercises.");

    public static Error DuplicateExerciseId => new("WorkoutPlan.DuplicateExerciseId", "WorkoutItems collection cannot contain duplicate ExerciseId.");
}
