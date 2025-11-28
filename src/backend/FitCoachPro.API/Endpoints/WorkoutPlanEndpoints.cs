using FitCoachPro.API.Endpoints.ApiRoutes;
using FitCoachPro.API.Filters;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.WorkoutPlan;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.API.Endpoints;

public static class WorkoutPlanEndpoints
{
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
            .RequireAuthorization(UserRole.Admin.ToString())
            .WithTags("Admin - Workout Plans");

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
            .RequireAuthorization(UserRole.Coach.ToString())
            .WithTags("Coach - Workout Plans");

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
            .RequireAuthorization(UserRole.Client.ToString())
            .WithTags("Client - Workout Plans");

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
           .RequireAuthorization(UserRole.Admin.ToString())
           .AddEndpointFilter<ValidationFilter<PaginationParams>>()
           .WithTags("Admin - Workout Plans");

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
           .RequireAuthorization(UserRole.Coach.ToString())
           .AddEndpointFilter<ValidationFilter<PaginationParams>>()
           .WithTags("Coach - Workout Plans");

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
            .RequireAuthorization(UserRole.Client.ToString())
            .AddEndpointFilter<ValidationFilter<PaginationParams>>()
            .WithTags("Client - Workout Plans");

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
            .RequireAuthorization(UserRole.Coach.ToString())
            .WithTags("Coach - Workout Plans");

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
            .RequireAuthorization(UserRole.Coach.ToString())
            .WithTags("Coach - Workout Plans");

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
            .RequireAuthorization(UserRole.Coach.ToString())
            .WithTags("Coach - Workout Plans");
    }
}
