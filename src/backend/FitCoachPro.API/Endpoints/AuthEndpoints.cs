using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Infrastructure.Services;

namespace FitCoachPro.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/auth");

        group.MapPost("sign-up", async (SignUpModel model, IAuthService authService, CancellationToken cancellationToken = default) =>
        {
            var response = await authService.SignUpAsync(model, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        });

        group.MapPost("sign-in", async (SignInModel model, IAuthService authService, CancellationToken cancellationToken = default) =>
        {
            var response = await authService.SignInAsync(model, cancellationToken);
            return Results.Json(response, statusCode: response.StatusCode);
        });
    } 
}
