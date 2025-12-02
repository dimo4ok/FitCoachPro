using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Common.Errors;

public static class TemplateWorkoutPlanErrors
{
    public static Error Forbidden => new("TemplateWorkoutPlan.Forbidden", "You do not have access to template.");
    public static Error NotFound => new("TemplateWorkoutPlan.NotFound", "Template workout plan not found.");
    public static Error AlreadyExists => new("TemplateWorkoutPlan.AlreadyExists", "A template workout plan with this name already exists.");
}
