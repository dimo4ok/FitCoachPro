using FitCoachPro.API.Common;
using FitCoachPro.API.Endpoints.ApiRoutes;
using FitCoachPro.API.Filters;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Requests;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FitCoachPro.API.Endpoints;

public static class CoachClientRequestEndpoints
{
    public const string AdminRequest = "Admin - Request";
    public const string CoachRequest = "Coach - Request";
    public const string ClientRequest = "Client - Request";

    public static void MapCoachClientRequestEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(CoachClientRequestRoutes.Admin.GetById,
            async (
                Guid id,
                IClientCoachRequestService service,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await service.GetByIdAsync(id, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Admin)
            .WithTags(AdminRequest);

        app.MapGet(CoachClientRequestRoutes.Coach.GetById,
           async (
               Guid id,
               IClientCoachRequestService service,
               CancellationToken cancellationToken = default
           ) =>
           {
               var response = await service.GetByIdAsync(id, cancellationToken);
               return Results.Json(response, statusCode: response.StatusCode);
           })
           .RequireAuthorization(AuthorizationPolicies.Coach)
           .WithTags(CoachRequest);

        app.MapGet(CoachClientRequestRoutes.Client.GetById,
            async (
                Guid id,
                IClientCoachRequestService service,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await service.GetByIdAsync(id, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Client)
            .WithTags(ClientRequest);

        app.MapGet(CoachClientRequestRoutes.Admin.GetAll,
            async (
                Guid userId,
                CoachRequestStatus? status,
                [AsParameters] PaginationParams paginationParams,
                IClientCoachRequestService service,
                CancellationToken cancellationToken
            ) =>
            {
                var response = await service.GetAllForAdminAsync(userId, paginationParams, status, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
           .RequireAuthorization(AuthorizationPolicies.Admin)
           .AddEndpointFilter<ValidationFilter<PaginationParams>>()
           .WithTags(AdminRequest);

        app.MapGet(CoachClientRequestRoutes.Coach.GetAll,
            async (
                CoachRequestStatus? status,
                [AsParameters] PaginationParams paginationParams,
                IClientCoachRequestService service,
                CancellationToken cancellationToken
            ) =>
            {
                var response = await service.GetAllForCoachOrClientAsync(paginationParams, status, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
           .RequireAuthorization(AuthorizationPolicies.Coach)
           .AddEndpointFilter<ValidationFilter<PaginationParams>>()
           .WithTags(CoachRequest);

        app.MapGet(CoachClientRequestRoutes.Client.GetAll,
            async (
                CoachRequestStatus? status,
                [AsParameters] PaginationParams paginationParams,
                IClientCoachRequestService service,
                CancellationToken cancellationToken
            ) =>
            {
                var response = await service.GetAllForCoachOrClientAsync(paginationParams, status, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Client)
            .AddEndpointFilter<ValidationFilter<PaginationParams>>()
            .WithTags(ClientRequest);

        app.MapPost(CoachClientRequestRoutes.Client.Create,
            async (
                Guid coachId,
                IClientCoachRequestService service,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await service.CreateAsync(coachId, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Client)
            .WithTags(ClientRequest);

        app.MapPut(CoachClientRequestRoutes.Coach.Update,
            async (
                Guid id,
                CoachRequestStatus status,
                IClientCoachRequestService service,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await service.UpdateAsync(id, status, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(CoachRequest);

        app.MapDelete(CoachClientRequestRoutes.Client.Cancel,
            async (
                Guid id,
                IClientCoachRequestService service,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await service.CancelRequestAsync(id, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Client)
            .WithTags(ClientRequest);
    }
}
