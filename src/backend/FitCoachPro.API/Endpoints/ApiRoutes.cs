namespace FitCoachPro.API.Endpoints;

public static class ApiRoutes
{
    public static class Auth
    {
        private const string Base = "api/auth";
        public const string SignUp = $"{Base}/sign-up";
        public const string SignIn = $"{Base}/sign-in";
    }
}
