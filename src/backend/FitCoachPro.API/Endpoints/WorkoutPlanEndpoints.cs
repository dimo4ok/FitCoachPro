using FitCoachPro.API.Filters;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Models.WorkoutPlan;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Domain.Entities.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;

namespace FitCoachPro.API.Endpoints;

public static class WorkoutPlanEndpoints
{
    public static void MapWorkoutPlanEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.WorkoutPlan.GetById,
            async (
                Guid id,
                IWorkoutPlanService service,
                HttpContext context,
                CancellationToken canceletionToken = default
            ) =>
            {
                var currentUserModel = context.User.ToCurrentUser();
                var response = await service.GetByIdAsync(id, currentUserModel, canceletionToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization();

        app.MapGet(ApiRoutes.WorkoutPlan.GetMyPlans,
            async (
                IWorkoutPlanService service,
                HttpContext context,
                CancellationToken canceletionToken = default
            ) =>
            {
                var currentUserModel = context.User.ToCurrentUser();
                var response = await service.GetMyWorkoutPlansAsync(currentUserModel, canceletionToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Client}" });

        app.MapGet(ApiRoutes.WorkoutPlan.GetPlansByClient,
            async (
                Guid clientId,
                IWorkoutPlanService service,
                HttpContext context,
                CancellationToken canceletionToken = default
            ) =>
            {
                var currentUserModel = context.User.ToCurrentUser();
                var response = await service.GetClientWorkoutPlansAsync(clientId, currentUserModel, canceletionToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Coach}, {UserRole.Admin}" });

        app.MapPost(ApiRoutes.WorkoutPlan.Create,
            async (
                CreateWorkoutPlanModel model,
                IWorkoutPlanService service,
                HttpContext context,
                CancellationToken cancellationToken = default
            ) =>
            {
                var currentUserModel = context.User.ToCurrentUser();
                var response = await service.CreateAsync(model, currentUserModel, cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .AddEndpointFilter<ValidationFilter<CreateWorkoutPlanModel>>()
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Coach}" });

        app.MapPut(ApiRoutes.WorkoutPlan.Update,
            async (
                Guid id,
                UpdateWorkoutPlanModel model,
                IValidator<UpdateWorkoutPlanModel> validator,
                IWorkoutPlanService service,
                HttpContext context,
                CancellationToken cancellationToken = default
            ) =>
            {
                var result = await validator.ValidateAsync(model, cancellationToken);
                if (!result.IsValid)
                    return Results.BadRequest(result.ToDictionary());

                var currentUserModel = context.User.ToCurrentUser();
                var response = await service.UpdateAsync(id, model, currentUserModel, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .AddEndpointFilter<ValidationFilter<UpdateWorkoutPlanModel>>()
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Coach}" });

        app.MapDelete(ApiRoutes.WorkoutPlan.Delete,
            async (
                Guid id,
                IWorkoutPlanService service,
                HttpContext context,
                CancellationToken cancellationToken = default
            ) =>
            {
                var currentUserModel = context.User.ToCurrentUser();
                var response = await service.DeleteByIdAsync(id, currentUserModel, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{UserRole.Coach}, {UserRole.Admin}" });
    }
}
