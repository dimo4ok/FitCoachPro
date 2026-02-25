using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Workouts;
using Microsoft.AspNetCore.Http;

namespace FitCoachPro.Application.Commands.Exercsies.UpdateExercise;

public class UpdateExerciseCommandHandler(
    IUserContextService userContext,
    IExerciseRepository exerciseRepository,
    IUnitOfWork unitOfWork,
    IExerciseAccessService accessService
    ) : ICommandHandler<UpdateExerciseCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IExerciseAccessService _accessService = accessService;

    public async Task<Result> ExecuteAsync(UpdateExerciseCommand command, CancellationToken cancellationToken)
    {
        if (!_accessService.HasUserAccess(_userContext.Current.Role))
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var exercise = await _exerciseRepository.GetByIdAsync(command.Id, cancellationToken, track: true);
        if (exercise == null)
            return Result.Fail(DomainErrors.NotFound(nameof(Exercise)));

        if (await _exerciseRepository.ExistsByExerciseNameForAnotherIdAsync(command.Id, command.Model.ExerciseName.NormalizeValue(), cancellationToken))
            return Result.Fail(DomainErrors.AlreadyExists(nameof(Exercise)), StatusCodes.Status409Conflict);

        if (!await _accessService.CanModifyExerciseAsync(command.Id, _userContext.Current.Role, cancellationToken))
            return Result.Fail(DomainErrors.UsedInActiveEntity(nameof(Exercise)), StatusCodes.Status409Conflict);

        exercise.ExerciseName = command.Model.ExerciseName;
        exercise.GifUrl = command.Model.GifUrl;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
