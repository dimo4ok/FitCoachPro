namespace FitCoachPro.API.Endpoints.ApiRoutes;

public static class UserRoutes
{
    private const string ById = "/{id:guid}";

    public static class Admin
    {
        private const string BaseAdmin = "api/admin";

        public const string GetAllUsers = $"{BaseAdmin}/users";

        public const string GetMyAdminProfile = $"{BaseAdmin}/me";
        public const string GetAdminById = $"{BaseAdmin}/admins{ById}";
        public const string GetCoachById = $"{BaseAdmin}/coaches{ById}";
        public const string GetClientById = $"{BaseAdmin}/clients{ById}";

        public const string UpdateMyProfile = $"{BaseAdmin}/me";
        public const string UpdateMyPassword = $"{BaseAdmin}/me/password";
    }

    public static class Coach
    {
        private const string BaseCoach = "api/coach";

        public const string GetAllUsers = $"{BaseCoach}/users";

        public const string GetMyCoachProfile = $"{BaseCoach}/me";
        public const string GetMyClients = $"{BaseCoach}/clients";
        public const string GetClientById = $"{BaseCoach}/clients{ById}";

        public const string UpdateMyProfile = $"{BaseCoach}/me";
        public const string UpdateMyPassword = $"{BaseCoach}/me/password";

        public const string UpdateAcceptingStatus = $"{BaseCoach}/accepting-status";
        public const string UnassignClient = $"{BaseCoach}/clients{ById}/unassign";
        public const string Delete = $"{BaseCoach}";
    }

    public static class Client
    {
        private const string BaseClient = "api/client";

        public const string GetAllUsers = $"{BaseClient}/users";

        public const string GetMyClientProfile = $"{BaseClient}/me";
        public const string GetMyCoach = $"{BaseClient}/coach";
        public const string GetCoachById = $"{BaseClient}/coaches{ById}";

        public const string UpdateMyProfile = $"{BaseClient}/me";
        public const string UpdateMyPassword = $"{BaseClient}/me/password";

        public const string UnassignCoach = $"{BaseClient}/coach/unassign";
        public const string Delete = $"{BaseClient}";
    }
}
