using FitCoachPro.Application.Common.Validators.WorkoutPlanValidators;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Services;
using FitCoachPro.Infrastructure.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FitCoachPro.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(CreateWorkoutPlanModelValidator).Assembly);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IWorkoutPlanService, WorkoutPlanService>();

        return services;
    }
}
