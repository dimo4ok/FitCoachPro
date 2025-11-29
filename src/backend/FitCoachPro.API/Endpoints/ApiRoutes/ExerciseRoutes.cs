namespace FitCoachPro.API.Endpoints.ApiRoutes;

public static class ExerciseRoutes
{
    private const string ById = $"/{{id:guid}}";

    public static class Admin
    {
        private const string BaseAdmin = "api/admin/exercise";

        public const string GetAll = $"{BaseAdmin}";
        public const string GetById = $"{BaseAdmin}{ById}";

        public const string Create = BaseAdmin;
        public const string Update = $"{BaseAdmin}{ById}";
        public const string Delete = $"{BaseAdmin}{ById}";
    }

    public static class Coach
    {
        private const string BaseCoach = "api/coach/exercise";

        public const string GetAll = $"{BaseCoach}";
        public const string GetById = $"{BaseCoach}{ById}";

        public const string Create = BaseCoach;
        public const string Update = $"{BaseCoach}{ById}";
        public const string Delete = $"{BaseCoach}{ById}";
    }
}