namespace FitCoachPro.API.Endpoints.ApiRoutes;

public static class CoachClientRequestRoutes
{
    private const string ById = $"/{{id:guid}}";

    public static class Admin
    {
        private const string BaseAdmin = "api/admin/request";

        public const string GetAll = BaseAdmin;
        public const string GetById = $"{BaseAdmin}{ById}";
    }

    public static class Coach
    {
        private const string BaseCoach = "api/coach/request";

        public const string GetAll = BaseCoach;
        public const string GetById = $"{BaseCoach}{ById}";

        public const string Update = $"{BaseCoach}{ById}";
    }

    public static class Client
    {
        private const string BaseClient = "api/client/request";

        public const string GetAll = BaseClient;
        public const string GetById = $"{BaseClient}{ById}";

        public const string Create = BaseClient;
        public const string Cancel = $"{BaseClient}{ById}";
    }
}
