using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Workouts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Commands.Exercsies.DeleteExercise;

public class DeleteExerciseCommandHandler(
    IUserContextService userContext,
    IExerciseRepository exerciseRepository,
    IUnitOfWork unitOfWork,
    IExerciseAccessService accessService,
    ILogger<DeleteExerciseCommandHandler> logger
    ) : ICommandHandler<DeleteExerciseCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IExerciseAccessService _accessService = accessService;
    private readonly ILogger<DeleteExerciseCommandHandler> _logger = logger;

    public async Task<Result> ExecuteAsync(DeleteExerciseCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        _logger.LogInformation(
            "DeleteExercise attempt started. ExerciseId: {ExerciseId}, UserId: {UserId}, Role: {Role}",
            command.Id, currentUser.UserId, currentUser.Role);

        if (!_accessService.HasUserAccess(currentUser.Role))
        {
            _logger.LogWarning(
                "DeleteExercise forbidden. ExerciseId: {ExerciseId}, UserId: {UserId}, Role: {Role}",
                command.Id, currentUser.UserId, currentUser.Role);
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        var exercise = await _exerciseRepository.GetByIdAsync(command.Id, cancellationToken, track: true);
        if (exercise == null)
        {
            _logger.LogWarning(
                "DeleteExercise failed: Exercise not found. ExerciseId: {ExerciseId}",
                command.Id);
            return Result.Fail(DomainErrors.NotFound(nameof(Exercise)));
        }

        if (!await _accessService.CanModifyExerciseAsync(command.Id, currentUser.Role, cancellationToken))
        {
            _logger.LogWarning(
                "DeleteExercise failed: Exercise is used in active entity. ExerciseId: {ExerciseId}",
                command.Id);
            return Result.Fail(DomainErrors.UsedInActiveEntity(nameof(Exercise)), StatusCodes.Status409Conflict);
        }

        _exerciseRepository.Delete(exercise);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "DeleteExercise succeeded. ExerciseId: {ExerciseId}, UserId: {UserId}",
            command.Id, currentUser.UserId);

        return Result.Success(StatusCodes.Status204NoContent);
    }
}
