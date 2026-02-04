using FitCoachPro.API.Common;
using FitCoachPro.API.Endpoints.ApiRoutes;
using FitCoachPro.API.Filters;
using FitCoachPro.Application.Commands.Users.UpdateMyProfile;
using FitCoachPro.Application.Commands.Users.UpdateMyProfilePassword;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Application.Queries.Users.Admins.GetAdminProfileById;
using FitCoachPro.Application.Queries.Users.Admins.GetMyAdminProfile;
using FitCoachPro.Application.Queries.Users.Clients.GetClientProfileById;
using FitCoachPro.Application.Queries.Users.Coaches.GetCoachProfileById;
using FitCoachPro.Application.Queries.Users.GetAllUsersByRole;
using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.API.Endpoints;

public static class AdminEndpoints
{
    public const string Admin = "Admin - User";

    public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(UserRoutes.Admin.GetAllUsers,
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
            .RequireAuthorization(AuthorizationPolicies.Admin)
            .AddEndpointFilter<ValidationFilter<PaginationParams>>()
            .WithTags(Admin);

        app.MapGet(UserRoutes.Admin.GetMyAdminProfile,
            async (
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetMyAdminProfileQuery,
                    Result<AdminPrivateProfileModel>>(
                        new GetMyAdminProfileQuery(),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Admin)
            .WithTags(Admin);

        app.MapGet(UserRoutes.Admin.GetAdminById,
            async (
                Guid id,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteQueryAsync<
                    GetAdminProfileByIdQuery,
                    Result<AdminPublicProfileModel>>(
                        new GetAdminProfileByIdQuery(id),
                        cancellationToken);

                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Admin)
            .WithTags(Admin);

        app.MapGet(UserRoutes.Admin.GetCoachById,
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
            .RequireAuthorization(AuthorizationPolicies.Admin)
            .WithTags(Admin);

        app.MapGet(UserRoutes.Admin.GetClientById,
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
            .RequireAuthorization(AuthorizationPolicies.Admin)
            .WithTags(Admin);

        app.MapPatch(UserRoutes.Admin.UpdateMyProfile,
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
            .RequireAuthorization(AuthorizationPolicies.Admin)
            .AddEndpointFilter<ValidationFilter<UpdateProfileModel>>()
            .WithTags(Admin);

        app.MapPatch(UserRoutes.Admin.UpdateMyPassword,
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
            .RequireAuthorization(AuthorizationPolicies.Admin)
            .AddEndpointFilter<ValidationFilter<UpdatePasswordModel>>()
            .WithTags(Admin);
    }
}
