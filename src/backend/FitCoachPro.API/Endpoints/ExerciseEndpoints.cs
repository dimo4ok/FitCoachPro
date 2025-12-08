using FitCoachPro.API.Common;
using FitCoachPro.API.Endpoints.ApiRoutes;
using FitCoachPro.API.Filters;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Workouts.Exercise;
using FitCoachPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitCoachPro.API.Endpoints;

public static class ExerciseEndpoints
{
    public const string AdminExercise = "Admin - Exercise";
    public const string CoachExercise = "Coach - Exercise";
    
    public static void MapExerciseEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(ExerciseRoutes.Admin.GetById,
            async (
                Guid id,
                IExerciseService service,
                CancellationToken cancellationToken
            ) =>
            {
                var response = await service.GetByIdAsync(id, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Admin)
            .WithTags(AdminExercise);

        app.MapGet(ExerciseRoutes.Coach.GetById,
          async (
              Guid id,
              IExerciseService service,
              CancellationToken cancellationToken
          ) =>
          {
              var response = await service.GetByIdAsync(id, cancellationToken);
              return Results.Json(response, statusCode: response.StatusCode);
          })
          .RequireAuthorization(AuthorizationPolicies.Coach)
          .WithTags(CoachExercise);

        app.MapGet(ExerciseRoutes.Admin.GetAll,
           async (
               [AsParameters] PaginationParams paginationParams,
               IExerciseService service,
               CancellationToken cancellationToken
           ) =>
           {
               var response = await service.GetAllAsync(paginationParams, cancellationToken);
               return Results.Json(response, statusCode: response.StatusCode);
           })
           .RequireAuthorization(AuthorizationPolicies.Admin)
           .AddEndpointFilter<ValidationFilter<PaginationParams>>()
           .WithTags(AdminExercise);

        app.MapGet(ExerciseRoutes.Coach.GetAll,
            async (
                [AsParameters] PaginationParams paginationParams,
                IExerciseService service,
                CancellationToken cancellationToken
                ) =>
            {
                var response = await service.GetAllAsync(paginationParams, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .AddEndpointFilter<ValidationFilter<PaginationParams>>()
            .WithTags(CoachExercise);

        app.MapPost(ExerciseRoutes.Admin.Create,
           async (
               CreateExerciseModel model,
               IExerciseService service,
               CancellationToken cancellationToken = default
           ) =>
           {
               var response = await service.CreateAsync(model, cancellationToken);
               return Results.Json(response, statusCode: response.StatusCode);
           })
           .RequireAuthorization(AuthorizationPolicies.Admin)
           .AddEndpointFilter<ValidationFilter<CreateExerciseModel>>()
           .WithTags(AdminExercise);

        app.MapPost(ExerciseRoutes.Coach.Create,
           async (
               CreateExerciseModel model,
               IExerciseService service,
               CancellationToken cancellationToken = default
           ) =>
           {
               var response = await service.CreateAsync(model, cancellationToken);
               return Results.Json(response, statusCode: response.StatusCode);
           })
           .RequireAuthorization(AuthorizationPolicies.Coach)
           .AddEndpointFilter<ValidationFilter<CreateExerciseModel>>()
           .WithTags(CoachExercise);

        app.MapPut(ExerciseRoutes.Admin.Update,
           async (
               Guid id,
               UpdateExerciseModel model,
               IExerciseService service,
               CancellationToken cancellationToken = default
           ) =>
           {
               var response = await service.UpdateAsync(id, model, cancellationToken);
               return Results.Json(response, statusCode: response.StatusCode);
           })
           .RequireAuthorization(AuthorizationPolicies.Admin)
           .AddEndpointFilter<ValidationFilter<UpdateExerciseModel>>()
           .WithTags(AdminExercise);

        app.MapPut(ExerciseRoutes.Coach.Update,
            async (
                Guid id,
                UpdateExerciseModel model,
                IExerciseService service,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await service.UpdateAsync(id, model, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .AddEndpointFilter<ValidationFilter<UpdateExerciseModel>>()
            .WithTags(CoachExercise);

        app.MapDelete(ExerciseRoutes.Admin.Delete,
            async (
                Guid id,
                [FromBody] DeleteExerciseModel model,
                IExerciseService service,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await service.DeleteAsync(id, model, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Admin)
            .AddEndpointFilter<ValidationFilter<DeleteExerciseModel>>()
            .WithTags(AdminExercise);

        app.MapDelete(ExerciseRoutes.Coach.Delete,
            async (
                Guid id,
                [FromBody] DeleteExerciseModel model,
                IExerciseService service,
                CancellationToken cancellationToken = default
            ) =>
            {
                var response = await service.DeleteAsync(id, model, cancellationToken);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .RequireAuthorization(AuthorizationPolicies.Coach)
            .AddEndpointFilter<ValidationFilter<DeleteExerciseModel>>()
            .WithTags(CoachExercise);
    }
}
