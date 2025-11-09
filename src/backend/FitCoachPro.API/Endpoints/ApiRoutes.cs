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
        private const string Base = "api/workout-plan";

        public const string GetAll = Base;
        public const string GetById = $"{Base}/{{id:guid}}";
        public const string Create = Base;
        public const string Delete = $"{Base}/{{id:guid}}";
    }
}
