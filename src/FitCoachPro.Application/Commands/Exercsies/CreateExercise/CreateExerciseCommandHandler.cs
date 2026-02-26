using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Extensions.WorkoutExtensions;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Workouts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Commands.Exercsies.CreateExercise;

public class CreateExerciseCommandHandler(
    IUserContextService userContext,
    IExerciseRepository exerciseRepository,
    IUnitOfWork unitOfWork,
    IExerciseAccessService accessService,
    ILogger<CreateExerciseCommandHandler> logger
    ) : ICommandHandler<CreateExerciseCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IExerciseAccessService _accessService = accessService;
    private readonly ILogger<CreateExerciseCommandHandler> _logger = logger;

    public async Task<Result> ExecuteAsync(CreateExerciseCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        var normalizedExerciseName = command.Model.ExerciseName.NormalizeValue();

        _logger.LogInformation(
            "CreateExercise attempt started. UserId: {UserId}, Role: {Role}, ExerciseName: {ExerciseName}",
            currentUser.UserId, currentUser.Role, command.Model.ExerciseName);

        if (!_accessService.HasUserAccess(currentUser.Role))
        {
            _logger.LogWarning(
                "CreateExercise forbidden. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        if (await _exerciseRepository.ExistsByExerciseNameAsync(normalizedExerciseName, cancellationToken))
        {
            _logger.LogWarning(
                "CreateExercise failed: Exercise already exists. ExerciseName: {ExerciseName}",
                command.Model.ExerciseName);
            return Result.Fail(DomainErrors.AlreadyExists(nameof(Exercise)), StatusCodes.Status409Conflict);
        }

        await _exerciseRepository.CreateAsync(command.Model.ToEntity(), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "CreateExercise succeeded. UserId: {UserId}, ExerciseName: {ExerciseName}",
            currentUser.UserId, command.Model.ExerciseName);

        return Result.Success(StatusCodes.Status201Created);
    }
}
