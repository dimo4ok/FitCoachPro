using FitCoachPro.Application.Commands.WorkoutPlans.DeleteWorkoutPlan;
using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using Microsoft.AspNetCore.Http;

namespace FitCoachPro.Application.Commands.WorkoutPlans.DeleteWorkoutPlan
{
    public class DeleteWorkoutPlanCommandHandler(
        IUserContextService userContext,
        IWorkoutPlanRepository workoutPlanRepository,
        IUnitOfWork unitOfWork,
        IWorkoutPlanAccessService workoutPlanAccessService
        ) : ICommandHandler<DeleteWorkoutPlanCommand, Result>
    {
        private readonly IUserContextService _userContext = userContext;
        private readonly IWorkoutPlanRepository _workoutPlanRepository = workoutPlanRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IWorkoutPlanAccessService _workoutPlanAccessService = workoutPlanAccessService;

        public async Task<Result> ExecuteAsync(DeleteWorkoutPlanCommand command, CancellationToken cancellationToken)
        {
            var workoutPlan = await _workoutPlanRepository.GetByIdAsync(command.Id, cancellationToken, track: true);
            if (workoutPlan == null)
                return Result.Fail(DomainErrors.NotFound(nameof(WorkoutPlan)));

            if (!await _workoutPlanAccessService.HasCoachAccessToWorkoutPlan(_userContext.Current, workoutPlan.ClientId, cancellationToken))
                return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

            _workoutPlanRepository.Delete(workoutPlan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(StatusCodes.Status204NoContent);
        }
    }
}
