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

namespace FitCoachPro.Application.Commands.Exercsies.CreateExercise;

public class CreateExerciseCommandHandler(
    IUserContextService userContext,
    IExerciseRepository exerciseRepository,
    IUnitOfWork unitOfWork,
    IExerciseAccessService accessService
    ) : ICommandHandler<CreateExerciseCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IExerciseAccessService _accessService = accessService;

    public async Task<Result> ExecuteAsync(CreateExerciseCommand command, CancellationToken cancellationToken)
    {
        if (!_accessService.HasUserAccess(_userContext.Current.Role))
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        if (await _exerciseRepository.ExistsByExerciseNameAsync(command.Model.ExerciseName.NormalizeValue(), cancellationToken))
            return Result.Fail(DomainErrors.AlreadyExists(nameof(Exercise)), StatusCodes.Status409Conflict);

        await _exerciseRepository.CreateAsync(command.Model.ToEntity(), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(StatusCodes.Status201Created);
    }
}
