using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Workouts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Commands.Exercsies.UpdateExercise;

public class UpdateExerciseCommandHandler(
    IUserContextService userContext,
    IExerciseRepository exerciseRepository,
    IUnitOfWork unitOfWork,
    IExerciseAccessService accessService,
    ILogger<UpdateExerciseCommandHandler> logger
    ) : ICommandHandler<UpdateExerciseCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IExerciseAccessService _accessService = accessService;
    private readonly ILogger<UpdateExerciseCommandHandler> _logger = logger;

    public async Task<Result> ExecuteAsync(UpdateExerciseCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        var normalizedExerciseName = command.Model.ExerciseName.NormalizeValue();

        _logger.LogInformation(
            "UpdateExercise attempt started. ExerciseId: {ExerciseId}, UserId: {UserId}, Role: {Role}, NewName: {ExerciseName}",
            command.Id, currentUser.UserId, currentUser.Role, command.Model.ExerciseName);

        if (!_accessService.HasUserAccess(currentUser.Role))
        {
            _logger.LogWarning(
                "UpdateExercise forbidden. ExerciseId: {ExerciseId}, UserId: {UserId}, Role: {Role}",
                command.Id, currentUser.UserId, currentUser.Role);
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        var exercise = await _exerciseRepository.GetByIdAsync(command.Id, cancellationToken, track: true);
        if (exercise == null)
        {
            _logger.LogWarning(
                "UpdateExercise failed: Exercise not found. ExerciseId: {ExerciseId}",
                command.Id);
            return Result.Fail(DomainErrors.NotFound(nameof(Exercise)));
        }

        if (await _exerciseRepository.ExistsByExerciseNameForAnotherIdAsync(command.Id, normalizedExerciseName, cancellationToken))
        {
            _logger.LogWarning(
                "UpdateExercise failed: Duplicate name. ExerciseId: {ExerciseId}, ExerciseName: {ExerciseName}",
                command.Id, command.Model.ExerciseName);
            return Result.Fail(DomainErrors.AlreadyExists(nameof(Exercise)), StatusCodes.Status409Conflict);
        }

        if (!await _accessService.CanModifyExerciseAsync(command.Id, _userContext.Current.Role, cancellationToken))
        {
            _logger.LogWarning(
                "UpdateExercise failed: Exercise is used in active entity. ExerciseId: {ExerciseId}",
                command.Id);
            return Result.Fail(DomainErrors.UsedInActiveEntity(nameof(Exercise)), StatusCodes.Status409Conflict);
        }

        exercise.ExerciseName = command.Model.ExerciseName;
        exercise.GifUrl = command.Model.GifUrl;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "UpdateExercise succeeded. ExerciseId: {ExerciseId}, UserId: {UserId}",
            command.Id, currentUser.UserId);

        return Result.Success();
    }
}
