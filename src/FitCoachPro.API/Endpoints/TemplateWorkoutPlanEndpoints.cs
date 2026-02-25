using FitCoachPro.API.Common;
using FitCoachPro.API.Endpoints.ApiRoutes;
using FitCoachPro.API.Filters;
using FitCoachPro.Application.Commands.TemplateWorkoutPlans.CreateTemplateWorkoutPlan;
using FitCoachPro.Application.Commands.TemplateWorkoutPlans.DeleteTemplateWorkoutPlan;
using FitCoachPro.Application.Commands.TemplateWorkoutPlans.UpdateTemplateWorkoutPlan;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Workouts.TemplateWorkoutPlan;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Application.Queries.TemplateWorkoutPlans.GetAllTempalatesForAdminByCoachId;
using FitCoachPro.Application.Queries.TemplateWorkoutPlans.GetAllTemplatesForCoach;
using FitCoachPro.Application.Queries.TemplateWorkoutPlans.GetTemplateById;

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
                IMediator mediator,
                CancellationToken canceletionToken = default
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetTemplateByIdQuery,
                    Result<TemplateWorkoutPlanModel>>(
                        new GetTemplateByIdQuery(id),
                        canceletionToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Admin)
            .WithTags(AdminTempalteWorkoutPLan);

        app.MapGet(TemplateWorkoutPlanRoutes.Coach.GetById,
            async (
                Guid id,
                IMediator mediator,
                CancellationToken canceletionToken = default
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetTemplateByIdQuery,
                    Result<TemplateWorkoutPlanModel>>(
                        new GetTemplateByIdQuery(id),
                        canceletionToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(CoachTemplateWorkoutPLan);

        app.MapGet(TemplateWorkoutPlanRoutes.Admin.GetAll,
            async (
                Guid coachId,
                [AsParameters] PaginationParams paginationParams,
                IMediator mediator,
                CancellationToken canceletionToken
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetAllTempalatesForAdminByCoachIdQuery,
                    Result<PaginatedModel<TemplateWorkoutPlanModel>>>(
                        new GetAllTempalatesForAdminByCoachIdQuery(coachId, paginationParams),
                        canceletionToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
           .RequireAuthorization(AuthorizationPolicies.Admin)
           .WithTags(AdminTempalteWorkoutPLan)
           .AddEndpointFilter<ValidationFilter<PaginationParams>>();

        app.MapGet(TemplateWorkoutPlanRoutes.Coach.GetAll,
            async (
                [AsParameters] PaginationParams paginationParams,
                IMediator mediator,
                CancellationToken canceletionToken
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetAllTemplatesForCoachQuery,
                    Result<PaginatedModel<TemplateWorkoutPlanModel>>>(
                        new GetAllTemplatesForCoachQuery(paginationParams),
                        canceletionToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
           .RequireAuthorization(AuthorizationPolicies.Coach)
           .WithTags(CoachTemplateWorkoutPLan)
           .AddEndpointFilter<ValidationFilter<PaginationParams>>();

        app.MapPost(TemplateWorkoutPlanRoutes.Coach.Create,
            async (
                CreateTemplateWorkoutPlanModel model,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteCommandAsync<
                    CreateTemplateCommand,
                    Result>(
                        new CreateTemplateCommand(model),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(CoachTemplateWorkoutPLan)
            .AddEndpointFilter<ValidationFilter<CreateTemplateWorkoutPlanModel>>();

        app.MapPut(TemplateWorkoutPlanRoutes.Coach.Update,
            async (
                Guid id,
                UpdateTemplateWorkoutPlanModel model,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteCommandAsync<
                    UpdateTemplateCommand,
                    Result>(
                    new UpdateTemplateCommand(id, model),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(CoachTemplateWorkoutPLan)
            .AddEndpointFilter<ValidationFilter<UpdateTemplateWorkoutPlanModel>>();

        app.MapDelete(TemplateWorkoutPlanRoutes.Coach.Delete,
            async (
                Guid id,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteCommandAsync<
                    DeleteTemplateCommand,
                    Result>(
                        new DeleteTemplateCommand(id),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(CoachTemplateWorkoutPLan);
    }
}
