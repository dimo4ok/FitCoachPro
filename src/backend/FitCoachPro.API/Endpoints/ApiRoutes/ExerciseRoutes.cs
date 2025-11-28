namespace FitCoachPro.API.Endpoints.ApiRoutes;

public static class ExerciseRoutes
{
    public static class Admin
    {
        private const string BaseAdmin = "api/admin/exercise";

        public const string GetAll = $"{BaseAdmin}";
        public const string GetById = $"{BaseAdmin}/{{id:guid}}";

        public const string Create = BaseAdmin;
        public const string Update = $"{BaseAdmin}/{{id:guid}}";
        public const string Delete = $"{BaseAdmin}/{{id:guid}}";
    }

    public static class Coach
    {
        private const string BaseCoach = "api/coach/exercise";

        public const string GetAll = $"{BaseCoach}";
        public const string GetById = $"{BaseCoach}/{{id:guid}}";

        public const string Create = BaseCoach;
        public const string Update = $"{BaseCoach}/{{id:guid}}";
        public const string Delete = $"{BaseCoach}/{{id:guid}}";
    }
}
