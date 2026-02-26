using FitCoachPro.Application.Commands.WorkoutPlans.DeleteWorkoutPlan;
using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Commands.WorkoutPlans.DeleteWorkoutPlan
{
    public class DeleteWorkoutPlanCommandHandler(
        IUserContextService userContext,
        IWorkoutPlanRepository workoutPlanRepository,
        IUnitOfWork unitOfWork,
        IWorkoutPlanAccessService workoutPlanAccessService,
        ILogger<DeleteWorkoutPlanCommandHandler> logger
        ) : ICommandHandler<DeleteWorkoutPlanCommand, Result>
    {
        private readonly IUserContextService _userContext = userContext;
        private readonly IWorkoutPlanRepository _workoutPlanRepository = workoutPlanRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IWorkoutPlanAccessService _workoutPlanAccessService = workoutPlanAccessService;
        private readonly ILogger<DeleteWorkoutPlanCommandHandler> _logger = logger;

        public async Task<Result> ExecuteAsync(DeleteWorkoutPlanCommand command, CancellationToken cancellationToken)
        {
            var currentUser = _userContext.Current;

            _logger.LogInformation(
                "DeleteWorkoutPlan attempt started. WorkoutPlanId: {WorkoutPlanId}, CoachId: {CoachId}",
                command.Id, currentUser.UserId);

            var workoutPlan = await _workoutPlanRepository.GetByIdAsync(command.Id, cancellationToken, track: true);
            if (workoutPlan == null)
            {
                _logger.LogWarning(
                    "DeleteWorkoutPlan failed: WorkoutPlan not found. WorkoutPlanId: {WorkoutPlanId}",
                    command.Id);
                return Result.Fail(DomainErrors.NotFound(nameof(WorkoutPlan)));
            }

            if (!await _workoutPlanAccessService.HasCoachAccessToWorkoutPlan(currentUser, workoutPlan.ClientId, cancellationToken))
            {
                _logger.LogWarning(
                    "DeleteWorkoutPlan forbidden. WorkoutPlanId: {WorkoutPlanId}, CoachId: {CoachId}, ClientId: {ClientId}",
                    command.Id, currentUser.UserId, workoutPlan.ClientId);
                return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
            }

            _workoutPlanRepository.Delete(workoutPlan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "DeleteWorkoutPlan succeeded. WorkoutPlanId: {WorkoutPlanId}, CoachId: {CoachId}, ClientId: {ClientId}",
                command.Id, currentUser.UserId, workoutPlan.ClientId);

            return Result.Success(StatusCodes.Status204NoContent);
        }
    }
}
