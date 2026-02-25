using FitCoachPro.API.Common;
using FitCoachPro.API.Endpoints.ApiRoutes;
using FitCoachPro.API.Filters;
using FitCoachPro.Application.Commands.ClientCoachRequests.CancelClientCoachRequest;
using FitCoachPro.Application.Commands.ClientCoachRequests.CreateClientCoachRequest;
using FitCoachPro.Application.Commands.ClientCoachRequests.UpdateClientCoachRequest;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Requests;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Application.Queries.ClientCoachRequests.GetAllClientCoachRequestsForCoachOrClient;
using FitCoachPro.Application.Queries.ClientCoachRequests.GetAllForAdmin;
using FitCoachPro.Application.Queries.ClientCoachRequests.GetClientCoachRequestById;
using FitCoachPro.Domain.Entities.Enums;

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
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetClientCoachRequestByIdQuery,
                    Result<ClientCoachRequestModel>>(
                        new GetClientCoachRequestByIdQuery(id),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Admin)
            .WithTags(AdminRequest);

        app.MapGet(CoachClientRequestRoutes.Coach.GetById,
            async (
                Guid id,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetClientCoachRequestByIdQuery,
                    Result<ClientCoachRequestModel>>(
                        new GetClientCoachRequestByIdQuery(id),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(CoachRequest);

        app.MapGet(CoachClientRequestRoutes.Client.GetById,
            async (
                Guid id,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetClientCoachRequestByIdQuery,
                    Result<ClientCoachRequestModel>>(
                        new GetClientCoachRequestByIdQuery(id),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Client)
            .WithTags(ClientRequest);

        app.MapGet(CoachClientRequestRoutes.Admin.GetAll,
            async (
                Guid userId,
                CoachRequestStatus? status,
                [AsParameters] PaginationParams paginationParams,
                IMediator mediator,
                CancellationToken cancellationToken
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetAllClientCoachRequestsForAdminQuery,
                    Result<PaginatedModel<ClientCoachRequestModel>>>(
                        new GetAllClientCoachRequestsForAdminQuery(userId, paginationParams, status),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
           .RequireAuthorization(AuthorizationPolicies.Admin)
           .AddEndpointFilter<ValidationFilter<PaginationParams>>()
           .WithTags(AdminRequest);

        app.MapGet(CoachClientRequestRoutes.Coach.GetAll,
            async (
                CoachRequestStatus? status,
                [AsParameters] PaginationParams paginationParams,
                IMediator mediator,
                CancellationToken cancellationToken
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetAllClientCoachRequestsForCoachOrClientQuery,
                    Result<PaginatedModel<ClientCoachRequestModel>>>(
                        new GetAllClientCoachRequestsForCoachOrClientQuery(paginationParams, status),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
           .RequireAuthorization(AuthorizationPolicies.Coach)
           .AddEndpointFilter<ValidationFilter<PaginationParams>>()
           .WithTags(CoachRequest);

        app.MapGet(CoachClientRequestRoutes.Client.GetAll,
            async (
                CoachRequestStatus? status,
                [AsParameters] PaginationParams paginationParams,
                IMediator mediator,
                CancellationToken cancellationToken
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetAllClientCoachRequestsForCoachOrClientQuery,
                    Result<PaginatedModel<ClientCoachRequestModel>>>(
                        new GetAllClientCoachRequestsForCoachOrClientQuery(paginationParams, status),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Client)
            .AddEndpointFilter<ValidationFilter<PaginationParams>>()
            .WithTags(ClientRequest);

        app.MapPost(CoachClientRequestRoutes.Client.Create,
            async (
                Guid coachId,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteCommandAsync<
                    CreateClientCoachRequestCommand,
                    Result>(
                        new CreateClientCoachRequestCommand(coachId),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Client)
            .WithTags(ClientRequest);

        app.MapPut(CoachClientRequestRoutes.Coach.Update,
            async (
                Guid id,
                CoachRequestStatus status,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteCommandAsync<
                    UpdateClientCoachRequestCommand,
                    Result>(
                        new UpdateClientCoachRequestCommand(id, status),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(CoachRequest);

        app.MapDelete(CoachClientRequestRoutes.Client.Cancel,
            async (
                Guid id,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteCommandAsync<
                   CancelClientCoachRequestCommand,
                    Result>(
                        new CancelClientCoachRequestCommand(id),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Client)
            .WithTags(ClientRequest);
    }
}
