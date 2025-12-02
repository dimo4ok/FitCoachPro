using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Common.Errors;

public static class TemplateWorkoutPlanErrors
{
    public static Error Forbidden => new("TemplateWorkoutPlan.Forbidden", "You do not have access to template.");
    public static Error NotFound => new("TemplateWorkoutPlan.NotFound", "Template workout plan not found.");
    public static Error AlreadyExists => new("TemplateWorkoutPlan.AlreadyExists", "A template workout plan with this name already exists.");

    public static Error TemplateNameRequired => new("TemplateWorkoutPlan.TemplateNameRequired", "Template name cannot be empty.");
    public static Error InvalidTemplateNameLength => new("TemplateWorkoutPlan.InvalidNameLength", "Template name must be between 3 and 50 characters.");

    public static Error DuplicateWorkoutItemId => new("TemplateWorkoutPlan.DuplicateId", "WorkoutItems collection cannot contain duplicate Ids.");
    public static Error NotEnoughItems => new("TemplateWorkoutPlan.NotEnoughItems", "Workout must include at least one exercise.");
    public static Error TooManyItems => new("TemplateWorkoutPlan.TooManyItems", "Workout cannot include more than 10 exercises.");

    public static Error DuplicateExerciseId => new("TemplateWorkoutPlan.DuplicateExerciseId", "WorkoutItems collection cannot contain duplicate ExerciseId.");
}
