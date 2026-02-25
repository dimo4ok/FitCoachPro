using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions.WorkoutExtensions;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Helpers;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using Microsoft.AspNetCore.Http;
namespace FitCoachPro.Application.Commands.WorkoutPlans.CreateWorkoutPlan;

public class CreateWorkoutPlanCommandHandler(
  IUserContextService userContext,
    IWorkoutPlanRepository workoutPlanRepository,
    IExerciseRepository exerciseRepository,
    IUnitOfWork unitOfWork,
    IWorkoutPlanHelper workoutPlanHelper,
    IWorkoutPlanAccessService workoutPlanAccessService
        ) : ICommandHandler<CreateWorkoutPlanCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IWorkoutPlanRepository _workoutPlanRepository = workoutPlanRepository;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IWorkoutPlanHelper _workoutPlanHelper = workoutPlanHelper;
    private readonly IWorkoutPlanAccessService _workoutPlanAccessService = workoutPlanAccessService;

    public async Task<Result> ExecuteAsync(CreateWorkoutPlanCommand command, CancellationToken cancellationToken)
    {
        if (!await _workoutPlanAccessService.HasCoachAccessToWorkoutPlan(_userContext.Current, command.Model.ClientId, cancellationToken))
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        if (await _workoutPlanRepository.ExistsByClientAndDateAsync(command.Model.ClientId, command.Model.WorkoutDate, cancellationToken))
            return Result.Fail(DomainErrors.AlreadyExists(nameof(WorkoutPlan)), StatusCodes.Status409Conflict);

        var exerciseIdsSet = _exerciseRepository
           .GetAllAsQuery()
           .Select(exercise => exercise.Id)
           .ToHashSet();
        if (exerciseIdsSet.Count == 0)
            return Result.Fail(DomainErrors.NotFound(nameof(Exercise)));

        var (exercsieExistSuccess, exercsieExistError) = _workoutPlanHelper.ExercisesExist(command.Model.WorkoutItems, exerciseIdsSet);
        if (!exercsieExistSuccess)
            return Result.Fail(exercsieExistError!, StatusCodes.Status400BadRequest);

        await _workoutPlanRepository.CreateAsync(command.Model.ToEntity(), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(StatusCodes.Status201Created);
    }
}
