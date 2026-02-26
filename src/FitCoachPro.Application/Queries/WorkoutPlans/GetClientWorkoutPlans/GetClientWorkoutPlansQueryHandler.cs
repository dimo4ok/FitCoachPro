using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Extensions.WorkoutExtensions;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Workouts.WorkoutPlan;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Queries.WorkoutPlans.GetClientWorkoutPlans;

public class GetClientWorkoutPlansQueryHandler(
    IUserContextService userContext,
    IWorkoutPlanRepository workoutPlanRepository,
    IWorkoutPlanAccessService workoutPlanAccessService,
    ILogger<GetClientWorkoutPlansQueryHandler> logger
    ) : IQueryHandler<GetClientWorkoutPlansQuery, Result<PaginatedModel<WorkoutPlanModel>>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IWorkoutPlanRepository _workoutPlanRepository = workoutPlanRepository;
    private readonly IWorkoutPlanAccessService _workoutPlanAccessService = workoutPlanAccessService;
    private readonly ILogger<GetClientWorkoutPlansQueryHandler> _logger = logger;

    public async Task<Result<PaginatedModel<WorkoutPlanModel>>> ExecuteAsync(GetClientWorkoutPlansQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        if (currentUser.Role != UserRole.Admin && !await _workoutPlanAccessService.HasCoachAccessToWorkoutPlan(currentUser, query.ClientId, cancellationToken))
        {
            _logger.LogWarning(
                "GetClientWorkoutPlans forbidden. CurrentUserId: {UserId}, Role: {Role}, ClientId: {ClientId}",
                currentUser.UserId, currentUser.Role, query.ClientId);
            return Result<PaginatedModel<WorkoutPlanModel>>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        _logger.LogInformation(
            "GetClientWorkoutPlans attempt started. CurrentUserId: {UserId}, Role: {Role}, ClientId: {ClientId}, Page: {PageNumber}, Size: {PageSize}",
            currentUser.UserId, currentUser.Role, query.ClientId, query.PaginationParams.PageNumber, query.PaginationParams.PageSize);

        var plansQuery = _workoutPlanRepository.GetAllByUserIdAsQuery(query.ClientId);
        if (!await plansQuery.AnyAsync(cancellationToken))
        {
            _logger.LogWarning(
                "GetClientWorkoutPlans failed: No workout plans found. ClientId: {ClientId}",
                query.ClientId);
            return Result<PaginatedModel<WorkoutPlanModel>>.Fail(DomainErrors.NotFound(nameof(WorkoutPlan)));
        }

        var paginated = await plansQuery.PaginateAsync(query.PaginationParams.PageNumber, query.PaginationParams.PageSize, cancellationToken);

        _logger.LogInformation(
            "GetClientWorkoutPlans succeeded. ClientId: {ClientId}, ReturnedCount: {Count}, Total: {Total}",
            query.ClientId, paginated.Items.Count, paginated.TotalItems);

        return Result<PaginatedModel<WorkoutPlanModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }
}
