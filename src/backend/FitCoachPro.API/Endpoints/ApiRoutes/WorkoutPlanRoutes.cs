namespace FitCoachPro.API.Endpoints.ApiRoutes;

public static class WorkoutPlanRoutes
{
    private const string ById = $"/{{id:guid}}";

    public static class Admin
    {
        private const string BaseAdmin = "api/admin/workout-plan";

        public const string GetAll = BaseAdmin;
        public const string GetById = $"{BaseAdmin}{ById}";

        public const string Create = BaseAdmin;
        public const string Update = $"{BaseAdmin}{ById}";
        public const string Delete = $"{BaseAdmin}{ById}";
    }

    public static class Coach
    {
        private const string BaseCoach = "api/coach/workout-plan";

        public const string GetAll = BaseCoach;
        public const string GetById = $"{BaseCoach}{ById}";

        public const string Create = BaseCoach;
        public const string Update = $"{BaseCoach}{ById}";
        public const string Delete = $"{BaseCoach}{ById}";
    }

    public static class Client
    {
        private const string BaseClient = "api/client/workout-plan";

        public const string GetAll = BaseClient;
        public const string GetById = $"{BaseClient}{ById}";

        public const string Create = BaseClient;
        public const string Update = $"{BaseClient}{ById}";
        public const string Delete = $"{BaseClient}{ById}";
    }
}
