using FitCoachPro.Application.Common.Validators.WorkoutPlanValidators;
using FitCoachPro.Application.Helpers;
using FitCoachPro.Application.Interfaces.Helpers;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Application.Services;
using FitCoachPro.Application.Services.Access;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FitCoachPro.Application.Common.ServiceRegistration;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IMediator, Mediator.Mediator>().AddMediatorHandlers();

        //helpers
        services.AddScoped<IAuthHelper, AuthHelper>();
        services.AddScoped<IWorkoutPlanHelper, WorkoutPlanHelper>();
        services.AddScoped<ITemplateWorkoutPlanHelper, TemplateWorkoutPlanHelper>();
        services.AddScoped<IUserHelper, UserHelper>();

        //services
        services.AddScoped<IUserContextService, UserContextService>();

        services.AddScoped<IWorkoutPlanAccessService, WorkoutPlanAccessService>();
        services.AddScoped<ITemplateWorkoutPlanAccessService, TemplateWorkoutPlanAccessService>();
        services.AddScoped<IExerciseAccessService, ExerciseAccessService>();
        services.AddScoped<IClientCoachRequestAccessService, ClientCoachRequestAccessService>();

        //validator
        services.AddValidatorsFromAssembly(typeof(CreateWorkoutPlanModelValidator).Assembly);

        services.AddHttpContextAccessor();

        return services;
    }
}
