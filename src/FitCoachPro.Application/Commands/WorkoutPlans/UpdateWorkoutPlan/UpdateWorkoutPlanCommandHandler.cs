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

namespace FitCoachPro.Application.Commands.WorkoutPlans.UpdateWorkoutPlan
{
    public class UpdateWorkoutPlanCommandHandler(
        IUserContextService userContext,
        IWorkoutPlanRepository workoutPlanRepository,
        IExerciseRepository exerciseRepository,
        IUnitOfWork unitOfWork,
        IWorkoutPlanHelper workoutPlanHelper,
        IWorkoutPlanAccessService workoutPlanAccessService
        ) : ICommandHandler<UpdateWorkoutPlanCommand, Result>
    {
        private readonly IUserContextService _userContext = userContext;
        private readonly IWorkoutPlanRepository _workoutPlanRepository = workoutPlanRepository;
        private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IWorkoutPlanHelper _workoutPlanHelper = workoutPlanHelper;
        private readonly IWorkoutPlanAccessService _workoutPlanAccessService = workoutPlanAccessService;

        public async Task<Result> ExecuteAsync(UpdateWorkoutPlanCommand command, CancellationToken cancellationToken)
        {
            var workoutPlan = await _workoutPlanRepository.GetByIdAsync(command.WorkoutPlanId, cancellationToken, track: true);
            if (workoutPlan == null)
                return Result.Fail(DomainErrors.NotFound(nameof(WorkoutPlan)));

            if (!await _workoutPlanAccessService.HasCoachAccessToWorkoutPlan(_userContext.Current, workoutPlan.ClientId, cancellationToken))
                return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

            if (await _workoutPlanRepository.ExistsByClientAndDateAsync(workoutPlan.ClientId, command.Model.WorkoutDate, cancellationToken)
                && command.Model.WorkoutDate != workoutPlan.WorkoutDate)
                return Result.Fail(DomainErrors.AlreadyExists(nameof(WorkoutPlan)), StatusCodes.Status409Conflict);

            workoutPlan.WorkoutDate = command.Model.WorkoutDate;

            var exerciseIdsSet = _exerciseRepository
               .GetAllAsQuery()
               .Select(exercise => exercise.Id)
               .ToHashSet();
            if (exerciseIdsSet.Count == 0)
                return Result.Fail(DomainErrors.NotFound(nameof(Exercise)));

            var (validateItemsSuccess, validateItemsError) = _workoutPlanHelper.ValidateUpdateItems(workoutPlan.WorkoutItems, command.Model.WorkoutItems, exerciseIdsSet);
            if (validateItemsSuccess == false)
                return Result.Fail(validateItemsError!, StatusCodes.Status400BadRequest);

            _workoutPlanHelper.SyncItems(workoutPlan.WorkoutItems, command.Model.WorkoutItems);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
