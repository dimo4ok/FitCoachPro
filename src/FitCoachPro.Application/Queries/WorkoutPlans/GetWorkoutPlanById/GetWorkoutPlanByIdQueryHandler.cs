using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions.WorkoutExtensions;
using FitCoachPro.Application.Common.Models.Workouts.WorkoutPlan;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Queries.WorkoutPlans.GetWorkoutPlanById;

public class GetWorkoutPlanByIdQueryHandler(
    IUserContextService userContext,
    IWorkoutPlanRepository workoutPlanRepository,
    IWorkoutPlanAccessService workoutPlanAccessService,
    ILogger<GetWorkoutPlanByIdQueryHandler> logger
    ) : IQueryHandler<GetWorkoutPlanByIdQuery, Result<WorkoutPlanModel>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IWorkoutPlanRepository _workoutPlanRepository = workoutPlanRepository;
    private readonly IWorkoutPlanAccessService _workoutPlanAccessService = workoutPlanAccessService;
    private readonly ILogger<GetWorkoutPlanByIdQueryHandler> _logger = logger;


    public async Task<Result<WorkoutPlanModel>> ExecuteAsync(GetWorkoutPlanByIdQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        _logger.LogInformation(
            "GetWorkoutPlanById attempt started. WorkoutPlanId: {WorkoutPlanId}, UserId: {UserId}, Role: {Role}",
            query.Id, currentUser.UserId, currentUser.Role);

        var workoutPlan = await _workoutPlanRepository.GetByIdAsync(query.Id, cancellationToken);
        if (workoutPlan == null)
        {
            _logger.LogWarning(
                "GetWorkoutPlanById failed: WorkoutPlan not found. WorkoutPlanId: {WorkoutPlanId}",
                query.Id);
            return Result<WorkoutPlanModel>.Fail(DomainErrors.NotFound(nameof(WorkoutPlan)));
        }

        if (!await _workoutPlanAccessService.HasUserAccessToWorkoutPlanAsync(currentUser, workoutPlan.ClientId, cancellationToken))
        {
            _logger.LogWarning(
                "GetWorkoutPlanById forbidden. WorkoutPlanId: {WorkoutPlanId}, UserId: {UserId}, Role: {Role}, ClientId: {ClientId}",
                query.Id, currentUser.UserId, currentUser.Role, workoutPlan.ClientId);
            return Result<WorkoutPlanModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        _logger.LogInformation(
            "GetWorkoutPlanById succeeded. WorkoutPlanId: {WorkoutPlanId}, ClientId: {ClientId}",
            workoutPlan.Id, workoutPlan.ClientId);

        return Result<WorkoutPlanModel>.Success(workoutPlan.ToModel());
    }
}
