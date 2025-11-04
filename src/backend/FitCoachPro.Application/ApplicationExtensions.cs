using FitCoachPro.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FitCoachPro.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
