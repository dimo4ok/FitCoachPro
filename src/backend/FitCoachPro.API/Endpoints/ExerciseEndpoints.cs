using FitCoachPro.API.Endpoints.ApiRoutes;
using FitCoachPro.API.Filters;
using FitCoachPro.Application.Common.Models.Exercise;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Domain.Entities.Enums;
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
            .RequireAuthorization(UserRole.Admin.ToString())
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
          .RequireAuthorization(UserRole.Coach.ToString())
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
           .RequireAuthorization(UserRole.Admin.ToString())
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
            .RequireAuthorization(UserRole.Coach.ToString())
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
           .RequireAuthorization(UserRole.Admin.ToString())
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
           .RequireAuthorization(UserRole.Coach.ToString())
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
           .RequireAuthorization(UserRole.Admin.ToString())
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
            .RequireAuthorization(UserRole.Coach.ToString())
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
            .RequireAuthorization(UserRole.Admin.ToString())
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
            .RequireAuthorization(UserRole.Coach.ToString())
            .AddEndpointFilter<ValidationFilter<DeleteExerciseModel>>()
            .WithTags(CoachExercise);
    }
}
