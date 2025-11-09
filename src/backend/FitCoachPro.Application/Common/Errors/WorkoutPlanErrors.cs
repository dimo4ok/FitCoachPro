
using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Common.Errors;

public class WorkoutPlanErrors
{
    public static Error NotFound => new("WorkoutPlan.NotFound", "Workout plan not found.");
    public static Error Forbidden => new("WokroutPlan.Forbidden", "You do not have access to this workout plan.");
}
