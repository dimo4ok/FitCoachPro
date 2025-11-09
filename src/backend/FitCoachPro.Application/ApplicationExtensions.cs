using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Services;
using FitCoachPro.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FitCoachPro.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IWorkoutPlanService, WorkoutPlanService>();

        return services;
    }
}
