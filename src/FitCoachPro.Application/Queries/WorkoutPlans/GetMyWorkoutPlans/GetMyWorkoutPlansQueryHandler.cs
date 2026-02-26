using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Extensions.WorkoutExtensions;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Workouts.WorkoutPlan;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Queries.WorkoutPlans.GetMyWorkoutPlans;

public class GetMyWorkoutPlansQueryHandler(
    IUserContextService userContext,
    IWorkoutPlanRepository workoutPlanRepository,
    ILogger<GetMyWorkoutPlansQueryHandler> logger
    ) : IQueryHandler<GetMyWorkoutPlansQuery, Result<PaginatedModel<WorkoutPlanModel>>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IWorkoutPlanRepository _workoutPlanRepository = workoutPlanRepository;
    private readonly ILogger<GetMyWorkoutPlansQueryHandler> _logger = logger;

    public async Task<Result<PaginatedModel<WorkoutPlanModel>>> ExecuteAsync(GetMyWorkoutPlansQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        _logger.LogInformation(
            "GetMyWorkoutPlans attempt started. UserId: {UserId}, Role: {Role}, Page: {PageNumber}, Size: {PageSize}",
            currentUser.UserId, currentUser.Role, query.PaginationParams.PageNumber, query.PaginationParams.PageSize);

        if (currentUser.Role != UserRole.Client)
        {
            _logger.LogWarning(
                "GetMyWorkoutPlans forbidden: User is not a Client. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result<PaginatedModel<WorkoutPlanModel>>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        var plansQuery = _workoutPlanRepository.GetAllByUserIdAsQuery(currentUser.UserId);
        if (!await plansQuery.AnyAsync(cancellationToken))
        {
            _logger.LogWarning(
                "GetMyWorkoutPlans failed: No workout plans found. ClientId: {ClientId}",
                currentUser.UserId);
            return Result<PaginatedModel<WorkoutPlanModel>>.Fail(DomainErrors.NotFound(nameof(WorkoutPlan)));
        }

        var paginated = await plansQuery.PaginateAsync(query.PaginationParams.PageNumber, query.PaginationParams.PageSize, cancellationToken);

        _logger.LogInformation(
            "GetMyWorkoutPlans succeeded. ClientId: {ClientId}, ReturnedCount: {Count}, Total: {Total}",
            currentUser.UserId, paginated.Items.Count, paginated.TotalItems);

        return Result<PaginatedModel<WorkoutPlanModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }
}
