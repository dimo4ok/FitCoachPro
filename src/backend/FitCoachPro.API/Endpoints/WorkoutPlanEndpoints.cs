using FitCoachPro.API.Common;
using FitCoachPro.API.Endpoints.ApiRoutes;
using FitCoachPro.API.Filters;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.WorkoutPlan;
using FitCoachPro.Application.Interfaces.Services;

namespace FitCoachPro.API.Endpoints;

public static class WorkoutPlanEndpoints
{
    public const string AdminWorkoutPLan = "Admin - Workout Plan";
    public const string CoachWorkoutPLan = "Coach - Workout Plan";
    public const string ClientWorkoutPLan = "Client - Workout Plan";

    public static void MapWorkoutPlanEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(WorkoutPlanRoutes.Admin.GetById,
            async (
                Guid id,
                IWorkoutPlanService service,
                CancellationToken canceletionToken = default
            ) =>
            {
                var response = await service.GetByIdAsync(id, canceletionToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Admin)
            .WithTags(AdminWorkoutPLan);

        app.MapGet(WorkoutPlanRoutes.Coach.GetById,
            async (
                Guid id,
                IWorkoutPlanService service,
                CancellationToken canceletionToken = default
            ) =>
            {
                var response = await service.GetByIdAsync(id, canceletionToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(CoachWorkoutPLan);

        app.MapGet(WorkoutPlanRoutes.Client.GetById,
            async (
                Guid id,
                IWorkoutPlanService service,
                CancellationToken canceletionToken = default
            ) =>
            {
                var response = await service.GetByIdAsync(id, canceletionToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Client)
            .WithTags(ClientWorkoutPLan);

        app.MapGet(WorkoutPlanRoutes.Admin.GetAll,
            async (
                Guid clientId,
                [AsParameters] PaginationParams paginationParams,
                IWorkoutPlanService service,
                CancellationToken canceletionToken
            ) =>
            {
                var response = await service.GetClientWorkoutPlansAsync(clientId, paginationParams, canceletionToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
           .RequireAuthorization(AuthorizationPolicies.Admin)
           .AddEndpointFilter<ValidationFilter<PaginationParams>>()
           .WithTags(AdminWorkoutPLan);

        app.MapGet(WorkoutPlanRoutes.Coach.GetAll,
            async (
                Guid clientId,
                [AsParameters] PaginationParams paginationParams,
                IWorkoutPlanService service,
                CancellationToken canceletionToken
            ) =>
            {
                var response = await service.GetClientWorkoutPlansAsync(clientId, paginationParams, canceletionToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
           .RequireAuthorization(AuthorizationPolicies.Coach)
           .AddEndpointFilter<ValidationFilter<PaginationParams>>()
           .WithTags(CoachWorkoutPLan);

        app.MapGet(WorkoutPlanRoutes.Client.GetAll,
            async (
                [AsParameters] PaginationParams paginationParams,
                IWorkoutPlanService service,
                CancellationToken cancellation
            ) =>
            {
                var response = await service.GetMyWorkoutPlansAsync(paginationParams, cancellation);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Client)
            .AddEndpointFilter<ValidationFilter<PaginationParams>>()
            .WithTags(ClientWorkoutPLan);

        app.MapPost(WorkoutPlanRoutes.Coach.Create,
            async (
                CreateWorkoutPlanModel model,
                IWorkoutPlanService service,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await service.CreateAsync(model, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .AddEndpointFilter<ValidationFilter<CreateWorkoutPlanModel>>()
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(CoachWorkoutPLan);

        app.MapPut(WorkoutPlanRoutes.Coach.Update,
            async (
                Guid id,
                UpdateWorkoutPlanModel model,
                IWorkoutPlanService service,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await service.UpdateAsync(id, model, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .AddEndpointFilter<ValidationFilter<UpdateWorkoutPlanModel>>()
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(CoachWorkoutPLan);

        app.MapDelete(WorkoutPlanRoutes.Coach.Delete,
            async (
                Guid id,
                IWorkoutPlanService service,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await service.DeleteAsync(id, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(CoachWorkoutPLan);
    }
}
