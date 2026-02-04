using FitCoachPro.API.Common;
using FitCoachPro.API.Endpoints.ApiRoutes;
using FitCoachPro.API.Filters;
using FitCoachPro.Application.Commands.Users.DeleteMyCoachAccount;
using FitCoachPro.Application.Commands.Users.UnassignClient;
using FitCoachPro.Application.Commands.Users.UpdateCoachAcceptingNewClients;
using FitCoachPro.Application.Commands.Users.UpdateMyProfile;
using FitCoachPro.Application.Commands.Users.UpdateMyProfilePassword;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Application.Queries.Users.Clients.GetClientProfileById;
using FitCoachPro.Application.Queries.Users.Coaches.GetCoachClients;
using FitCoachPro.Application.Queries.Users.Coaches.GetMyCoachProfile;
using FitCoachPro.Application.Queries.Users.GetAllUsersByRole;
using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.API.Endpoints;

public static class CoachEndpoints
{
    public const string Coach = "Coach - User";

    public static void MapCoachEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(UserRoutes.Coach.GetAllUsers,
            async (
                UserRole userRole,
                [AsParameters] PaginationParams paginationParams,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetAllUsersByRoleQuery,
                    Result<PaginatedModel<UserProfileModel>>>(
                        new GetAllUsersByRoleQuery(paginationParams, userRole),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .AddEndpointFilter<ValidationFilter<PaginationParams>>()
            .WithTags(Coach);

        app.MapGet(UserRoutes.Coach.GetMyCoachProfile,
            async (
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetMyCoachProfileQuery,
                    Result<CoachPrivateProfileModel>>(
                        new GetMyCoachProfileQuery(),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(Coach);

        app.MapGet(UserRoutes.Coach.GetClientById,
            async (
                Guid id,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetClientProfileByIdQuery,
                    Result<ClientPublicProfileModel>>(
                        new GetClientProfileByIdQuery(id),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(Coach);

        app.MapGet(UserRoutes.Coach.GetMyClients,
            async (
                [AsParameters] PaginationParams paginationParams,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetCoachClientsQuery,
                    Result<PaginatedModel<ClientPrivateProfileModel>>>(
                        new GetCoachClientsQuery(paginationParams),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(Coach);

        app.MapPatch(UserRoutes.Coach.UpdateMyProfile,
            async (
                UpdateProfileModel model,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteCommandAsync<
                    UpdateMyProfileCommand,
                    Result>(
                        new UpdateMyProfileCommand(model),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .AddEndpointFilter<ValidationFilter<UpdateProfileModel>>()
            .WithTags(Coach);

        app.MapPatch(UserRoutes.Coach.UpdateMyPassword,
            async (
                UpdatePasswordModel model,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteCommandAsync<
                    UpdateMyProfilePasswordCommand,
                    Result>(
                        new UpdateMyProfilePasswordCommand(model),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .AddEndpointFilter<ValidationFilter<UpdatePasswordModel>>()
            .WithTags(Coach);

        app.MapPatch(UserRoutes.Coach.UnassignClient,
            async (
                Guid id,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteCommandAsync<
                    UnassignClientCommand,
                    Result>(
                        new UnassignClientCommand(id),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(Coach);

        app.MapPatch(UserRoutes.Coach.UpdateAcceptingStatus,
            async (
                ClientAcceptanceStatus status,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteCommandAsync<
                    UpdateCoachAcceptingNewClientsCommand,
                    Result>(
                        new UpdateCoachAcceptingNewClientsCommand(status),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(Coach);

        app.MapDelete(UserRoutes.Coach.Delete,
            async (
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteCommandAsync<
                   DeleteMyCoachAccountCommand,
                   Result>(
                       new DeleteMyCoachAccountCommand(),
                       cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .WithTags(Coach);
    }
}
