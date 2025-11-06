using FitCoachPro.Application.Common.Models;
using FitCoachPro.Infrastructure.Services;

namespace FitCoachPro.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Auth.SignUp, async (SignUpModel model, IAuthService authService, CancellationToken cancellationToken = default) =>
        {
            var response = await authService.SignUpAsync(model, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        });

        app.MapPost(ApiRoutes.Auth.SignIn, async (SignInModel model, IAuthService authService, CancellationToken cancellationToken = default) =>
        {
            var response = await authService.SignInAsync(model, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        });
    } 
}
