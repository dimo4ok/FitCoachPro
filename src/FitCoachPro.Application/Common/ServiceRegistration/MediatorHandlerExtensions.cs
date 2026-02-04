using FitCoachPro.Application.Commands.Auth.SignIn;
using FitCoachPro.Application.Commands.Auth.SignUp;
using FitCoachPro.Application.Commands.ClientCoachRequests.CancelClientCoachRequest;
using FitCoachPro.Application.Commands.ClientCoachRequests.CreateClientCoachRequest;
using FitCoachPro.Application.Commands.ClientCoachRequests.DeleteOwnClientCoachRequests;
using FitCoachPro.Application.Commands.ClientCoachRequests.UpdateClientCoachRequest;
using FitCoachPro.Application.Commands.Exercsies.CreateExercise;
using FitCoachPro.Application.Commands.Exercsies.DeleteExercise;
using FitCoachPro.Application.Commands.Exercsies.UpdateExercise;
using FitCoachPro.Application.Commands.TemplateWorkoutPlans.CreateTemplateWorkoutPlan;
using FitCoachPro.Application.Commands.TemplateWorkoutPlans.DeleteTemplateWorkoutPlan;
using FitCoachPro.Application.Commands.TemplateWorkoutPlans.UpdateTemplateWorkoutPlan;
using FitCoachPro.Application.Commands.Users.DeleteMyClientAccount;
using FitCoachPro.Application.Commands.Users.DeleteMyCoachAccount;
using FitCoachPro.Application.Commands.Users.UnassignClient;
using FitCoachPro.Application.Commands.Users.UnassignCoach;
using FitCoachPro.Application.Commands.Users.UpdateCoachAcceptingNewClients;
using FitCoachPro.Application.Commands.Users.UpdateMyProfile;
using FitCoachPro.Application.Commands.Users.UpdateMyProfilePassword;
using FitCoachPro.Application.Commands.WorkoutPlans.CreateWorkoutPlan;
using FitCoachPro.Application.Commands.WorkoutPlans.DeleteWorkoutPlan;
using FitCoachPro.Application.Commands.WorkoutPlans.UpdateWorkoutPlan;
using FitCoachPro.Application.Common.Models.Auth;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Requests;
using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Application.Common.Models.Workouts.Exercise;
using FitCoachPro.Application.Common.Models.Workouts.TemplateWorkoutPlan;
using FitCoachPro.Application.Common.Models.Workouts.WorkoutPlan;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Application.Queries.ClientCoachRequests.GetAllClientCoachRequestsForCoachOrClient;
using FitCoachPro.Application.Queries.ClientCoachRequests.GetAllForAdmin;
using FitCoachPro.Application.Queries.ClientCoachRequests.GetClientCoachRequestById;
using FitCoachPro.Application.Queries.Exercsies.GetAllExercises;
using FitCoachPro.Application.Queries.Exercsies.GetExerciseById;
using FitCoachPro.Application.Queries.TemplateWorkoutPlans.GetAllTempalatesForAdminByCoachId;
using FitCoachPro.Application.Queries.TemplateWorkoutPlans.GetAllTemplatesForCoach;
using FitCoachPro.Application.Queries.TemplateWorkoutPlans.GetTemplateById;
using FitCoachPro.Application.Queries.Users.Admins.GetAdminProfileById;
using FitCoachPro.Application.Queries.Users.Admins.GetMyAdminProfile;
using FitCoachPro.Application.Queries.Users.Clients.GetClientCoach;
using FitCoachPro.Application.Queries.Users.Clients.GetClientProfileById;
using FitCoachPro.Application.Queries.Users.Clients.GetMyClientProfile;
using FitCoachPro.Application.Queries.Users.Coaches.GetCoachClients;
using FitCoachPro.Application.Queries.Users.Coaches.GetCoachProfileById;
using FitCoachPro.Application.Queries.Users.Coaches.GetMyCoachProfile;
using FitCoachPro.Application.Queries.Users.GetAllUsersByRole;
using FitCoachPro.Application.Queries.WorkoutPlans.GetClientWorkoutPlans;
using FitCoachPro.Application.Queries.WorkoutPlans.GetMyWorkoutPlans;
using FitCoachPro.Application.Queries.WorkoutPlans.GetWorkoutPlanById;
using Microsoft.Extensions.DependencyInjection;

namespace FitCoachPro.Application.Common.ServiceRegistration;

public static class MediatorHandlerExtensions
{
    public static IServiceCollection AddMediatorHandlers(this IServiceCollection services)
    {
        //auth
        services.AddScoped<ICommandHandler<SignInCommand, Result<AuthModel>>, SignInCommandHandler>();
        services.AddScoped<ICommandHandler<SignUpCommand, Result<AuthModel>>, SignUpCommandHandler>();

        //users
        services.AddScoped<IQueryHandler<GetAdminProfileByIdQuery, Result<AdminPublicProfileModel>>, GetAdminProfileByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetMyAdminProfileQuery, Result<AdminPrivateProfileModel>>, GetMyAdminProfileQueryHandler>();

        services.AddScoped<IQueryHandler<GetClientCoachQuery, Result<CoachPrivateProfileModel>>, GetClientCoachQueryHandler>();
        services.AddScoped<IQueryHandler<GetClientProfileByIdQuery, Result<ClientPublicProfileModel>>, GetClientProfileByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetMyClientProfileQuery, Result<ClientPrivateProfileModel>>, GetOwnClientProfileQueryHandler>();

        services.AddScoped<IQueryHandler<GetCoachClientsQuery, Result<PaginatedModel<ClientPrivateProfileModel>>>, GetCoachClientsQueryHandler>();
        services.AddScoped<IQueryHandler<GetCoachProfileByIdQuery, Result<CoachPublicProfileModel>>, GetCoachProfileByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetMyCoachProfileQuery, Result<CoachPrivateProfileModel>>, GetMyCoachProfileQueryHandler>();

        services.AddScoped<IQueryHandler<GetAllUsersByRoleQuery, Result<PaginatedModel<UserProfileModel>>>, GetAllUsersByRoleQueryHandler>();

        services.AddScoped<ICommandHandler<UnassignCoachCommand, Result>, UnassignCoachCommandHandler>();
        services.AddScoped<ICommandHandler<UnassignClientCommand, Result>, UnassignClientCommandHandler>();

        services.AddScoped<ICommandHandler<DeleteMyClientAccountCommand, Result>, DeleteMyClientAccountCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteMyCoachAccountCommand, Result>, DeleteMyCoachAccountCommandHandler>();

        services.AddScoped<ICommandHandler<UpdateMyProfileCommand, Result>, UpdateMyProfileCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateMyProfilePasswordCommand, Result>, UpdateMyProfilePasswordCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateCoachAcceptingNewClientsCommand, Result>, UpdateCoachAcceptingNewClientsCommandHandler>();

        //wokroutPlans
        services.AddScoped<IQueryHandler<GetWorkoutPlanByIdQuery, Result<WorkoutPlanModel>>, GetWorkoutPlanByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetClientWorkoutPlansQuery, Result<PaginatedModel<WorkoutPlanModel>>>, GetClientWorkoutPlansQueryHandler>();
        services.AddScoped<IQueryHandler<GetMyWorkoutPlansQuery, Result<PaginatedModel<WorkoutPlanModel>>>, GetMyWorkoutPlansQueryHandler>();

        services.AddScoped<ICommandHandler<CreateWorkoutPlanCommand, Result>, CreateWorkoutPlanCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateWorkoutPlanCommand, Result>, UpdateWorkoutPlanCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteWorkoutPlanCommand, Result>, DeleteWorkoutPlanCommandHandler>();

        //templateWorkoutPlans
        services.AddScoped<IQueryHandler<GetTemplateByIdQuery, Result<TemplateWorkoutPlanModel>>, GetTemplateByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetAllTemplatesForCoachQuery, Result<PaginatedModel<TemplateWorkoutPlanModel>>>, GetAllTemplatesForCoachQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetAllTempalatesForAdminByCoachIdQuery, Result<PaginatedModel<TemplateWorkoutPlanModel>>>,
            GetAllTempalatesForAdminByCoachIdQueryHandler>();

        services.AddScoped<ICommandHandler<CreateTemplateCommand, Result>, CreateTemplateCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateTemplateCommand, Result>, UpdateTemplateCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteTemplateCommand, Result>, DeleteTemplateCommandHandler>();

        //exercises
        services.AddScoped<IQueryHandler<GetExerciseByIdQuery, Result<ExerciseModel>>, GetExerciseByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetAllExercisesQuery, Result<PaginatedModel<ExerciseModel>>>, GetAllExercisesQueryHandler>();

        services.AddScoped<ICommandHandler<CreateExerciseCommand, Result>, CreateExerciseCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateExerciseCommand, Result>, UpdateExerciseCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteExerciseCommand, Result>, DeleteExerciseCommandHandler>();

        //coachClientRequests
        services.AddScoped<
            IQueryHandler<GetClientCoachRequestByIdQuery, Result<ClientCoachRequestModel>>,
            GetClientCoachRequestByIdQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetAllClientCoachRequestsForAdminQuery, Result<PaginatedModel<ClientCoachRequestModel>>>,
            GetAllClientCoachRequestsForAdminQueryHandler>();
        services.AddScoped<
            IQueryHandler<GetAllClientCoachRequestsForCoachOrClientQuery, Result<PaginatedModel<ClientCoachRequestModel>>>,
            GetAllClientCoachRequestsForCoachOrClientQueryHandler>();

        services.AddScoped<ICommandHandler<CreateClientCoachRequestCommand, Result>, CreateClientCoachRequestCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateClientCoachRequestCommand, Result>, UpdateClientCoachRequestCommandHandler>();
        services.AddScoped<ICommandHandler<CancelClientCoachRequestCommand, Result>, CancelClientCoachRequestCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteOwnClientCoachRequestsCommand, Result>, DeleteOwnClientCoachRequestsCommandHandler>();

        return services;
    }
}
