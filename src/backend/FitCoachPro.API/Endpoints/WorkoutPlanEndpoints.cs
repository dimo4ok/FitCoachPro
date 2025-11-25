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
        app.MapGet(ApiRoutes.WorkoutPlan.Admin.GetById,
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
            .WithTags("Workout Plans - Admin");

        app.MapGet(ApiRoutes.WorkoutPlan.Coach.GetById,
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
        .WithTags("Workout Plans - Coach");

        app.MapGet(ApiRoutes.WorkoutPlan.Client.GetById,
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
        .WithTags("Workout Plans - Client");

        app.MapGet(ApiRoutes.WorkoutPlan.Admin.GetAll,
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
           .WithTags("Workout Plans - Admin");

        app.MapGet(ApiRoutes.WorkoutPlan.Coach.GetAll,
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
           .WithTags("Workout Plans - Coach");

        app.MapGet(ApiRoutes.WorkoutPlan.Client.GetAll,
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
            .WithTags("Workout Plans - Client");

        app.MapPost(ApiRoutes.WorkoutPlan.Coach.Create,
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
            .WithTags("Workout Plans - Coach");

        app.MapPut(ApiRoutes.WorkoutPlan.Coach.Update,
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
            .WithTags("Workout Plans - Coach");

        app.MapDelete(ApiRoutes.WorkoutPlan.Coach.Delete,
            async (
                Guid id,
                IWorkoutPlanService service,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await service.DeleteByIdAsync(id, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(UserRole.Coach.ToString())
            .WithTags("Workout Plans - Coach");
    }
}
