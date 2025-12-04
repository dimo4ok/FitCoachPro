using FitCoachPro.API.Common;
using FitCoachPro.API.Endpoints.ApiRoutes;
using FitCoachPro.API.Filters;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.TemplateWorkoutPlan;
using FitCoachPro.Application.Interfaces.Services;

namespace FitCoachPro.API.Endpoints;

public static class TemplateWorkoutPlanEndpoints
{
    public const string AdminTempalteWorkoutPLan = "Admin - Template Workout Plan";
    public const string CoachTemplateWorkoutPLan = "Coach - Template Workout Plan";

    public static void MapTempalteWorkoutPlanEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(TemplateWorkoutPlanRoutes.Admin.GetById,
            async (
                Guid id,
                ITemplateWorkoutPlanService service,
                CancellationToken canceletionToken = default
            ) =>
            {
                var response = await service.GetByIdAsync(id, canceletionToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Admin)
            .WithTags(AdminTempalteWorkoutPLan);

        app.MapGet(TemplateWorkoutPlanRoutes.Coach.GetById,
            async (
                Guid id,
                ITemplateWorkoutPlanService service,
                CancellationToken canceletionToken = default
            ) =>
            {
                var response = await service.GetByIdAsync(id, canceletionToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(CoachTemplateWorkoutPLan);

        app.MapGet(TemplateWorkoutPlanRoutes.Admin.GetAll,
            async (
                Guid coachId,
                [AsParameters] PaginationParams paginationParams,
                ITemplateWorkoutPlanService service,
                CancellationToken canceletionToken
            ) =>
            {
                var response = await service.GetAllForAdminByCoachIdAsync(coachId, paginationParams, canceletionToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
           .RequireAuthorization(AuthorizationPolicies.Admin)
           .WithTags(AdminTempalteWorkoutPLan)
           .AddEndpointFilter<ValidationFilter<PaginationParams>>();

        app.MapGet(TemplateWorkoutPlanRoutes.Coach.GetAll,
            async (
                [AsParameters] PaginationParams paginationParams,
                ITemplateWorkoutPlanService service,
                CancellationToken canceletionToken
            ) =>
            {
                var response = await service.GetAllForCoachAsync(paginationParams, canceletionToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
           .RequireAuthorization(AuthorizationPolicies.Coach)
           .WithTags(CoachTemplateWorkoutPLan)
           .AddEndpointFilter<ValidationFilter<PaginationParams>>();

        app.MapPost(TemplateWorkoutPlanRoutes.Coach.Create,
            async (
                CreateTemplateWorkoutPlanModel model,
                ITemplateWorkoutPlanService service,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await service.CreateAsync(model, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(CoachTemplateWorkoutPLan)
            .AddEndpointFilter<ValidationFilter<CreateTemplateWorkoutPlanModel>>();

        app.MapPut(TemplateWorkoutPlanRoutes.Coach.Update,
            async (
                Guid id,
                UpdateTemplateWorkoutPlanModel model,
                ITemplateWorkoutPlanService service,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await service.UpdateAsync(id, model, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(CoachTemplateWorkoutPLan)
            .AddEndpointFilter<ValidationFilter<UpdateTemplateWorkoutPlanModel>>();

        app.MapDelete(TemplateWorkoutPlanRoutes.Coach.Delete,
            async (
                Guid id,
                ITemplateWorkoutPlanService service,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await service.DeleteAsync(id, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(CoachTemplateWorkoutPLan);
    }
}
