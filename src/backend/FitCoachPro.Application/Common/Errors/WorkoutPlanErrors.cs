
using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Common.Errors;

public class WorkoutPlanErrors
{
    public static Error NotFound => new("WorkoutPlan.NotFound", "Workout plan not found.");
    public static Error Forbidden => new("WorkoutPlan.Forbidden", "You do not have access to workout plans for this client.");
    public static Error AlreadyExists => new("WorkoutPlan.AlreadyExists", "A workout plan for this date already exists.");
}
