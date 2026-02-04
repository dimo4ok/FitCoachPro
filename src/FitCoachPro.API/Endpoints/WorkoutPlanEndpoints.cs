using FitCoachPro.API.Common;
using FitCoachPro.API.Endpoints.ApiRoutes;
using FitCoachPro.API.Filters;
using FitCoachPro.Application.Commands.WorkoutPlans.CreateWorkoutPlan;
using FitCoachPro.Application.Commands.WorkoutPlans.DeleteWorkoutPlan;
using FitCoachPro.Application.Commands.WorkoutPlans.UpdateWorkoutPlan;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Workouts.WorkoutPlan;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Application.Queries.WorkoutPlans.GetClientWorkoutPlans;
using FitCoachPro.Application.Queries.WorkoutPlans.GetMyWorkoutPlans;
using FitCoachPro.Application.Queries.WorkoutPlans.GetWorkoutPlanById;

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
                IMediator mediator,
                CancellationToken canceletionToken = default
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetWorkoutPlanByIdQuery,
                    Result<WorkoutPlanModel>>(
                        new GetWorkoutPlanByIdQuery(id),
                        canceletionToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Admin)
            .WithTags(AdminWorkoutPLan);

        app.MapGet(WorkoutPlanRoutes.Coach.GetById,
            async (
                Guid id,
                IMediator mediator,
                CancellationToken canceletionToken = default
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetWorkoutPlanByIdQuery,
                    Result<WorkoutPlanModel>>(
                        new GetWorkoutPlanByIdQuery(id),
                        canceletionToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(CoachWorkoutPLan);

        app.MapGet(WorkoutPlanRoutes.Client.GetById,
            async (
                Guid id,
                IMediator mediator,
                CancellationToken canceletionToken = default
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetWorkoutPlanByIdQuery,
                    Result<WorkoutPlanModel>>(
                        new GetWorkoutPlanByIdQuery(id),
                        canceletionToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Client)
            .WithTags(ClientWorkoutPLan);

        app.MapGet(WorkoutPlanRoutes.Admin.GetAll,
            async (
                Guid clientId,
                [AsParameters] PaginationParams paginationParams,
                IMediator mediator,
                CancellationToken canceletionToken
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetClientWorkoutPlansQuery,
                    Result<PaginatedModel<WorkoutPlanModel>>>(
                        new GetClientWorkoutPlansQuery(clientId, paginationParams),
                        canceletionToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
           .RequireAuthorization(AuthorizationPolicies.Admin)
           .AddEndpointFilter<ValidationFilter<PaginationParams>>()
           .WithTags(AdminWorkoutPLan);

        app.MapGet(WorkoutPlanRoutes.Coach.GetAll,
            async (
                Guid clientId,
                [AsParameters] PaginationParams paginationParams,
                IMediator mediator,
                CancellationToken canceletionToken
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetClientWorkoutPlansQuery,
                    Result<PaginatedModel<WorkoutPlanModel>>>(
                        new GetClientWorkoutPlansQuery(clientId, paginationParams),
                        canceletionToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
           .RequireAuthorization(AuthorizationPolicies.Coach)
           .AddEndpointFilter<ValidationFilter<PaginationParams>>()
           .WithTags(CoachWorkoutPLan);

        app.MapGet(WorkoutPlanRoutes.Client.GetAll,
            async (
                [AsParameters] PaginationParams paginationParams,
                IMediator mediator,
                CancellationToken cancellationToken
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetMyWorkoutPlansQuery,
                    Result<PaginatedModel<WorkoutPlanModel>>>(
                        new GetMyWorkoutPlansQuery(paginationParams),
                        cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Client)
            .AddEndpointFilter<ValidationFilter<PaginationParams>>()
            .WithTags(ClientWorkoutPLan);

        app.MapPost(WorkoutPlanRoutes.Coach.Create,
            async (
                CreateWorkoutPlanModel model,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteCommandAsync<
                    CreateWorkoutPlanCommand,
                    Result>(
                        new CreateWorkoutPlanCommand(model),
                        cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .AddEndpointFilter<ValidationFilter<CreateWorkoutPlanModel>>()
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(CoachWorkoutPLan);

        app.MapPut(WorkoutPlanRoutes.Coach.Update,
            async (
                Guid id,
                UpdateWorkoutPlanModel model,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteCommandAsync<
                    UpdateWorkoutPlanCommand,
                    Result>(
                        new UpdateWorkoutPlanCommand(id, model),
                        cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .AddEndpointFilter<ValidationFilter<UpdateWorkoutPlanModel>>()
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(CoachWorkoutPLan);

        app.MapDelete(WorkoutPlanRoutes.Coach.Delete,
            async (
                Guid id,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteCommandAsync<
                    DeleteWorkoutPlanCommand,
                    Result>(
                        new DeleteWorkoutPlanCommand(id),
                        cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(CoachWorkoutPLan);
    }
}
