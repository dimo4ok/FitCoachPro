namespace FitCoachPro.API.Endpoints;

public static class ApiRoutes
{
    public static class Auth
    {
        private const string Base = "api/auth";

        public const string SignUp = $"{Base}/sign-up";
        public const string SignIn = $"{Base}/sign-in";
    }

    public static class WorkoutPlan
    {
        public static class Admin
        {
            private const string BaseAdmin = "api/admin/workout-plan";

            public const string GetAll = BaseAdmin;
            public const string GetById = $"{BaseAdmin}/{{id:guid}}";

            public const string Create = BaseAdmin;
            public const string Update = $"{BaseAdmin}/{{id:guid}}";
            public const string Delete = $"{BaseAdmin}/{{id:guid}}";
        }

        public static class Coach
        {
            private const string BaseCoach = "api/coach/workout-plan";

            public const string GetAll = BaseCoach;
            public const string GetById = $"{BaseCoach}/{{id:guid}}";

            public const string Create = BaseCoach;
            public const string Update = $"{BaseCoach}/{{id:guid}}";
            public const string Delete = $"{BaseCoach}/{{id:guid}}";
        }

        public static class Client
        {
            private const string BaseClient = "api/client/workout-plan";

            public const string GetAll = BaseClient;
            public const string GetById = $"{BaseClient}/{{id:guid}}";

            public const string Create = BaseClient;
            public const string Update = $"{BaseClient}/{{id:guid}}";
            public const string Delete = $"{BaseClient}/{{id:guid}}";
        }
    }
}
