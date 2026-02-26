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
using Microsoft.Extensions.Logging;
namespace FitCoachPro.Application.Commands.WorkoutPlans.CreateWorkoutPlan;

public class CreateWorkoutPlanCommandHandler(
  IUserContextService userContext,
    IWorkoutPlanRepository workoutPlanRepository,
    IExerciseRepository exerciseRepository,
    IUnitOfWork unitOfWork,
    IWorkoutPlanHelper workoutPlanHelper,
    IWorkoutPlanAccessService workoutPlanAccessService,
    ILogger<CreateWorkoutPlanCommandHandler> logger
        ) : ICommandHandler<CreateWorkoutPlanCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IWorkoutPlanRepository _workoutPlanRepository = workoutPlanRepository;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IWorkoutPlanHelper _workoutPlanHelper = workoutPlanHelper;
    private readonly IWorkoutPlanAccessService _workoutPlanAccessService = workoutPlanAccessService;
    private readonly ILogger<CreateWorkoutPlanCommandHandler> _logger = logger;

    public async Task<Result> ExecuteAsync(CreateWorkoutPlanCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        _logger.LogInformation(
            "CreateWorkoutPlan attempt started. CoachId: {CoachId}, ClientId: {ClientId}, Date: {WorkoutDate}",
            currentUser.UserId, command.Model.ClientId, command.Model.WorkoutDate);

        if (!await _workoutPlanAccessService.HasCoachAccessToWorkoutPlan(currentUser, command.Model.ClientId, cancellationToken))
        {
            _logger.LogWarning(
                "CreateWorkoutPlan forbidden. CoachId: {CoachId}, ClientId: {ClientId}",
                currentUser.UserId, command.Model.ClientId);
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        if (await _workoutPlanRepository.ExistsByClientAndDateAsync(command.Model.ClientId, command.Model.WorkoutDate, cancellationToken))
        {
            _logger.LogWarning(
                "CreateWorkoutPlan failed: WorkoutPlan already exists. ClientId: {ClientId}, Date: {WorkoutDate}",
                command.Model.ClientId, command.Model.WorkoutDate);
            return Result.Fail(DomainErrors.AlreadyExists(nameof(WorkoutPlan)), StatusCodes.Status409Conflict);
        }

        var exerciseIdsSet = _exerciseRepository
           .GetAllAsQuery()
           .Select(exercise => exercise.Id)
           .ToHashSet();
        if (exerciseIdsSet.Count == 0)
        {
            _logger.LogWarning(
                "CreateWorkoutPlan failed: No exercises found in system. CoachId: {CoachId}",
                currentUser.UserId);
            return Result.Fail(DomainErrors.NotFound(nameof(Exercise)));
        }

        var (exercsieExistSuccess, exercsieExistError) = _workoutPlanHelper.ExercisesExist(command.Model.WorkoutItems, exerciseIdsSet);
        if (!exercsieExistSuccess)
        {
            _logger.LogWarning(
                "CreateWorkoutPlan failed: Invalid exercise ids in payload. CoachId: {CoachId}, ClientId: {ClientId}. Error: {@Error}",
                currentUser.UserId, command.Model.ClientId, exercsieExistError);
            return Result.Fail(exercsieExistError!, StatusCodes.Status400BadRequest);
        }

        await _workoutPlanRepository.CreateAsync(command.Model.ToEntity(), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "CreateWorkoutPlan succeeded. CoachId: {CoachId}, ClientId: {ClientId}, Date: {WorkoutDate}",
            currentUser.UserId, command.Model.ClientId, command.Model.WorkoutDate);

        return Result.Success(StatusCodes.Status201Created);
    }
}
