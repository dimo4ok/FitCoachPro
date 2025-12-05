using FitCoachPro.Application.Common.Validators.WorkoutPlanValidators;
using FitCoachPro.Application.Helpers;
using FitCoachPro.Application.Interfaces.Helpers;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Services;
using FitCoachPro.Application.Services.Workout;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FitCoachPro.Application.Common.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(CreateWorkoutPlanModelValidator).Assembly);

        services.AddHttpContextAccessor();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserContextService, UserContextService>();

        services.AddScoped<ITemplateWorkoutPlanService, TemplateWorkoutPlanService>();
        services.AddScoped<ITemplateWorkoutPlanHelper, TemplateWorkoutPlanHelper>();

        services.AddScoped<IWorkoutPlanService, WorkoutPlanService>();
        services.AddScoped<IWorkoutPlanHelper, WorkoutPlanHelper>();

        services.AddScoped<IExerciseService, ExerciseService>();

        return services;
    }
}
