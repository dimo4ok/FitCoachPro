using FitCoachPro.Application.Common.Errors;
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

namespace FitCoachPro.Application.Commands.WorkoutPlans.UpdateWorkoutPlan
{
    public class UpdateWorkoutPlanCommandHandler(
        IUserContextService userContext,
        IWorkoutPlanRepository workoutPlanRepository,
        IExerciseRepository exerciseRepository,
        IUnitOfWork unitOfWork,
        IWorkoutPlanHelper workoutPlanHelper,
        IWorkoutPlanAccessService workoutPlanAccessService,
        ILogger<UpdateWorkoutPlanCommandHandler> logger
        ) : ICommandHandler<UpdateWorkoutPlanCommand, Result>
    {
        private readonly IUserContextService _userContext = userContext;
        private readonly IWorkoutPlanRepository _workoutPlanRepository = workoutPlanRepository;
        private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IWorkoutPlanHelper _workoutPlanHelper = workoutPlanHelper;
        private readonly IWorkoutPlanAccessService _workoutPlanAccessService = workoutPlanAccessService;
        private readonly ILogger<UpdateWorkoutPlanCommandHandler> _logger = logger;

        public async Task<Result> ExecuteAsync(UpdateWorkoutPlanCommand command, CancellationToken cancellationToken)
        {
            var currentUser = _userContext.Current;

            _logger.LogInformation(
                "UpdateWorkoutPlan attempt started. WorkoutPlanId: {WorkoutPlanId}, CoachId: {CoachId}, NewDate: {WorkoutDate}",
                command.WorkoutPlanId, currentUser.UserId, command.Model.WorkoutDate);

            var workoutPlan = await _workoutPlanRepository.GetByIdAsync(command.WorkoutPlanId, cancellationToken, track: true);
            if (workoutPlan == null)
            {
                _logger.LogWarning(
                    "UpdateWorkoutPlan failed: WorkoutPlan not found. WorkoutPlanId: {WorkoutPlanId}",
                    command.WorkoutPlanId);
                return Result.Fail(DomainErrors.NotFound(nameof(WorkoutPlan)));
            }

            if (!await _workoutPlanAccessService.HasCoachAccessToWorkoutPlan(currentUser, workoutPlan.ClientId, cancellationToken))
            {
                _logger.LogWarning(
                    "UpdateWorkoutPlan forbidden. WorkoutPlanId: {WorkoutPlanId}, CoachId: {CoachId}, ClientId: {ClientId}",
                    command.WorkoutPlanId, currentUser.UserId, workoutPlan.ClientId);
                return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
            }

            if (await _workoutPlanRepository.ExistsByClientAndDateAsync(workoutPlan.ClientId, command.Model.WorkoutDate, cancellationToken)
                && command.Model.WorkoutDate != workoutPlan.WorkoutDate)
            {
                _logger.LogWarning(
                    "UpdateWorkoutPlan failed: WorkoutPlan already exists for date. WorkoutPlanId: {WorkoutPlanId}, ClientId: {ClientId}, Date: {WorkoutDate}",
                    command.WorkoutPlanId, workoutPlan.ClientId, command.Model.WorkoutDate);
                return Result.Fail(DomainErrors.AlreadyExists(nameof(WorkoutPlan)), StatusCodes.Status409Conflict);
            }

            workoutPlan.WorkoutDate = command.Model.WorkoutDate;

            var exerciseIdsSet = _exerciseRepository
               .GetAllAsQuery()
               .Select(exercise => exercise.Id)
               .ToHashSet();
            if (exerciseIdsSet.Count == 0)
            {
                _logger.LogWarning(
                    "UpdateWorkoutPlan failed: No exercises found in system. WorkoutPlanId: {WorkoutPlanId}",
                    command.WorkoutPlanId);
                return Result.Fail(DomainErrors.NotFound(nameof(Exercise)));
            }

            var (validateItemsSuccess, validateItemsError) = _workoutPlanHelper.ValidateUpdateItems(workoutPlan.WorkoutItems, command.Model.WorkoutItems, exerciseIdsSet);
            if (validateItemsSuccess == false)
            {
                _logger.LogWarning(
                    "UpdateWorkoutPlan failed: Invalid items update payload. WorkoutPlanId: {WorkoutPlanId}. Error: {@Error}",
                    command.WorkoutPlanId, validateItemsError);
                return Result.Fail(validateItemsError!, StatusCodes.Status400BadRequest);
            }

            _workoutPlanHelper.SyncItems(workoutPlan.WorkoutItems, command.Model.WorkoutItems);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "UpdateWorkoutPlan succeeded. WorkoutPlanId: {WorkoutPlanId}, ClientId: {ClientId}, Date: {WorkoutDate}",
                command.WorkoutPlanId, workoutPlan.ClientId, workoutPlan.WorkoutDate);

            return Result.Success();
        }
    }
}
