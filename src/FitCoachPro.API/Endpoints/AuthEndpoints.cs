using FitCoachPro.API.Endpoints.ApiRoutes;
using FitCoachPro.API.Filters;
using FitCoachPro.Application.Commands.Auth.SignIn;
using FitCoachPro.Application.Commands.Auth.SignUp;
using FitCoachPro.Application.Common.Models.Auth;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Mediator.Interfaces;

namespace FitCoachPro.API.Endpoints;

public static class AuthEndpoints
{
    private const string Auth = "1.Authentication";

    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost(AuthRoutes.SignUp,
            async (
                SignUpModel model,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteCommandAsync<SignUpCommand, Result<AuthModel>>(new SignUpCommand(model), cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .AddEndpointFilter<ValidationFilter<SignUpModel>>()
            .WithTags(Auth);

        app.MapPost(AuthRoutes.SignIn,
            async (
                SignInModel model,
                IMediator mediator,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await mediator.ExecuteCommandAsync<SignInCommand, Result<AuthModel>>(new SignInCommand(model), cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .AddEndpointFilter<ValidationFilter<SignInModel>>()
            .WithTags(Auth);
    }
}
