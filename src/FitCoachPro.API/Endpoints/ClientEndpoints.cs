using FitCoachPro.API.Common;
using FitCoachPro.API.Endpoints.ApiRoutes;
using FitCoachPro.API.Filters;
using FitCoachPro.Application.Commands.Users.DeleteMyClientAccount;
using FitCoachPro.Application.Commands.Users.UnassignCoach;
using FitCoachPro.Application.Commands.Users.UpdateMyProfile;
using FitCoachPro.Application.Commands.Users.UpdateMyProfilePassword;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Application.Queries.Users.Clients.GetClientCoach;
using FitCoachPro.Application.Queries.Users.Clients.GetMyClientProfile;
using FitCoachPro.Application.Queries.Users.Coaches.GetCoachProfileById;
using FitCoachPro.Application.Queries.Users.GetAllUsersByRole;
using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.API.Endpoints;

public static class ClientEndpoints
{
    public const string Client = "Client - User";
    
    public static void MapClientEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(UserRoutes.Client.GetAllUsers,
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
            .RequireAuthorization(AuthorizationPolicies.Client)
            .AddEndpointFilter<ValidationFilter<PaginationParams>>()
            .WithTags(Client);

        app.MapGet(UserRoutes.Client.GetMyClientProfile,
            async (
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetMyClientProfileQuery,
                    Result<ClientPrivateProfileModel>>(
                        new GetMyClientProfileQuery(),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Client)
            .WithTags(Client);

        app.MapGet(UserRoutes.Client.GetCoachById,
            async (
                Guid id,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetCoachProfileByIdQuery,
                    Result<CoachPublicProfileModel>>(
                        new GetCoachProfileByIdQuery(id),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Client)
            .WithTags(Client);

        app.MapGet(UserRoutes.Client.GetMyCoach,
            async (
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetClientCoachQuery,
                    Result<CoachPrivateProfileModel>>(
                        new GetClientCoachQuery(),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Client)
            .WithTags(Client);

        app.MapPatch(UserRoutes.Client.UpdateMyProfile,
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
            .RequireAuthorization(AuthorizationPolicies.Client)
            .AddEndpointFilter<ValidationFilter<UpdateProfileModel>>()
            .WithTags(Client);

        app.MapPatch(UserRoutes.Client.UpdateMyPassword,
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
            .RequireAuthorization(AuthorizationPolicies.Client)
            .AddEndpointFilter<ValidationFilter<UpdatePasswordModel>>()
            .WithTags(Client);

        app.MapPatch(UserRoutes.Client.UnassignCoach,
            async (
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteCommandAsync<
                    UnassignCoachCommand,
                    Result>(
                        new UnassignCoachCommand(),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Client)
            .WithTags(Client);

        app.MapDelete(UserRoutes.Client.Delete,
            async (
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteCommandAsync<
                   DeleteMyClientAccountCommand,
                   Result>(
                       new DeleteMyClientAccountCommand(),
                       cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Client)
            .WithTags(Client);
    }
}
