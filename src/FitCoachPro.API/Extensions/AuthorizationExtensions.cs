namespace FitCoachPro.API.Extensions
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddAppAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Coach", policy
                    => policy.RequireRole("Coach"));

                options.AddPolicy("Admin", policy
                    => policy.RequireRole("Admin"));

                options.AddPolicy("Client", policy
                    => policy.RequireRole("Client"));
            });

            return services;
        }
    }
}
