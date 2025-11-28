using FitCoachPro.API.Endpoints.ApiRoutes;
using FitCoachPro.API.Filters;
using FitCoachPro.Application.Common.Models.Auth;
using FitCoachPro.Application.Interfaces.Services;

namespace FitCoachPro.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost(AuthRoutes.SignUp,
            async (
                SignUpModel model,
                IAuthService authService,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await authService.SignUpAsync(model, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .AddEndpointFilter<ValidationFilter<SignUpModel>>()
            .WithTags("1.Authentication");

        app.MapPost(AuthRoutes.SignIn,
            async (
                SignInModel model,
                IAuthService authService,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await authService.SignInAsync(model, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .AddEndpointFilter<ValidationFilter<SignInModel>>()
            .WithTags("1.Authentication");
    }
}
