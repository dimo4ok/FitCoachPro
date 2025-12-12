using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions.WorkoutExtensions;
using FitCoachPro.Application.Common.Models;
using FitCoachPro.Application.Common.Models.Workouts.WorkoutPlan;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using Microsoft.AspNetCore.Http;

namespace FitCoachPro.Application.Queries.WorkoutPlans.GetWorkoutPlanById;

public class GetWorkoutPlanByIdQueryHandler(
    IUserContextService userContext,
    IWorkoutPlanRepository workoutPlanRepository,
    IUserRepository userRepository
    ) : IQueryHandler<GetWorkoutPlanByIdQuery, Result<WorkoutPlanModel>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IWorkoutPlanRepository _workoutPlanRepository = workoutPlanRepository;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result<WorkoutPlanModel>> ExecuteAsync(GetWorkoutPlanByIdQuery query, CancellationToken cancellationToken)
    {
        var workoutPlan = await _workoutPlanRepository.GetByIdAsync(query.Id, cancellationToken);
        if (workoutPlan == null)
            return Result<WorkoutPlanModel>.Fail(DomainErrors.NotFound(nameof(WorkoutPlan)));

        if (!await HasUserAccessToWorkoutPlanAsync(_userContext.Current, workoutPlan.ClientId, cancellationToken))
            return Result<WorkoutPlanModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        return Result<WorkoutPlanModel>.Success(workoutPlan.ToModel());
    }

    private async Task<bool> HasUserAccessToWorkoutPlanAsync(UserContext currentUser, Guid clientId, CancellationToken cancellationToken) =>
       currentUser.Role switch
       {
           UserRole.Client => clientId == currentUser.UserId,
           UserRole.Coach => await _userRepository.CanCoachAccessClientAsync(currentUser.UserId, clientId, cancellationToken),
           UserRole.Admin => true,
           _ => false
       };
}
