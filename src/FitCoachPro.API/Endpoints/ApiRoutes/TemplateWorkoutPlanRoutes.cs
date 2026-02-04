namespace FitCoachPro.API.Endpoints.ApiRoutes;

public static class TemplateWorkoutPlanRoutes
{
    private const string ById = $"/{{id:guid}}";

    public static class Admin
    {
        private const string BaseAdmin = "api/admin/template-workout-plan";

        public const string GetAll = BaseAdmin;
        public const string GetById = $"{BaseAdmin}{ById}";
    }

    public static class Coach
    {
        private const string BaseCoach = "api/coach/template-workout-plan";

        public const string GetAll = BaseCoach;
        public const string GetById = $"{BaseCoach}{ById}";

        public const string Create = BaseCoach;
        public const string Update = $"{BaseCoach}{ById}";
        public const string Delete = $"{BaseCoach}{ById}";
    }
}
