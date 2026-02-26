using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Extensions.WorkoutExtensions;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Workouts.Exercise;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Workouts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Queries.Exercsies.GetAllExercises;

public class GetAllExercisesQueryHandler(
    IUserContextService userContext,
    IExerciseRepository exerciseRepository,
    IExerciseAccessService accessService,
    ILogger<GetAllExercisesQueryHandler> logger
    ) : IQueryHandler<GetAllExercisesQuery, Result<PaginatedModel<ExerciseModel>>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
    private readonly IExerciseAccessService _accessService = accessService;
    private readonly ILogger<GetAllExercisesQueryHandler> _logger = logger;

    public async Task<Result<PaginatedModel<ExerciseModel>>> ExecuteAsync(GetAllExercisesQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        _logger.LogInformation(
            "GetAllExercises attempt started. UserId: {UserId}, Role: {Role}, Page: {PageNumber}, Size: {PageSize}",
            currentUser.UserId, currentUser.Role, query.PaginationParams.PageNumber, query.PaginationParams.PageSize);

        if (!_accessService.HasUserAccess(currentUser.Role))
        {
            _logger.LogWarning(
                "GetAllExercises forbidden. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result<PaginatedModel<ExerciseModel>>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        var exercsiesQuery = _exerciseRepository.GetAllAsQuery();
        if (!await exercsiesQuery.AnyAsync(cancellationToken))
        {
            _logger.LogWarning(
                "GetAllExercises failed: No exercises found.");
            return Result<PaginatedModel<ExerciseModel>>.Fail(DomainErrors.NotFound(nameof(Exercise)));
        }

        var paginated = await exercsiesQuery.PaginateAsync(query.PaginationParams.PageNumber, query.PaginationParams.PageSize, cancellationToken);

        _logger.LogInformation(
            "GetAllExercises succeeded. ReturnedCount: {Count}, Total: {Total}",
            paginated.Items.Count, paginated.TotalItems);

        return Result<PaginatedModel<ExerciseModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }
}