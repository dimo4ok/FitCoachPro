using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions.WorkoutExtensions;
using FitCoachPro.Application.Common.Models.Workouts.Exercise;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Workouts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Queries.Exercsies.GetExerciseById;

public class GetExerciseByIdQueryHandler(
    IUserContextService userContext,
    IExerciseRepository exerciseRepository,
    IExerciseAccessService accessService,
    ILogger<GetExerciseByIdQueryHandler> logger
    ) : IQueryHandler<GetExerciseByIdQuery, Result<ExerciseModel>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
    private readonly IExerciseAccessService _accessService = accessService;
    private readonly ILogger<GetExerciseByIdQueryHandler> _logger = logger;

    public async Task<Result<ExerciseModel>> ExecuteAsync(GetExerciseByIdQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        _logger.LogInformation(
            "GetExerciseById attempt started. ExerciseId: {ExerciseId}, UserId: {UserId}, Role: {Role}",
            query.Id, currentUser.UserId, currentUser.Role);

        if (!_accessService.HasUserAccess(currentUser.Role))
        {
            _logger.LogWarning(
                "GetExerciseById forbidden. ExerciseId: {ExerciseId}, UserId: {UserId}, Role: {Role}",
                query.Id, currentUser.UserId, currentUser.Role);
            return Result<ExerciseModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        var exercise = await _exerciseRepository.GetByIdAsync(query.Id, cancellationToken);
        if (exercise == null)
        {
            _logger.LogWarning(
                "GetExerciseById failed: Exercise not found. ExerciseId: {ExerciseId}",
                query.Id);
            return Result<ExerciseModel>.Fail(DomainErrors.NotFound(nameof(Exercise)));
        }

        _logger.LogInformation(
            "GetExerciseById succeeded. ExerciseId: {ExerciseId}",
            query.Id);

        return Result<ExerciseModel>.Success(exercise.ToModel());
    }
}