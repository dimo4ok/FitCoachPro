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

namespace FitCoachPro.Application.Queries.WorkoutPlans.GetMyWorkoutPlans;

public class GetMyWorkoutPlansQueryHandler(
    IUserContextService userContext,
    IWorkoutPlanRepository workoutPlanRepository
    ) : IQueryHandler<GetMyWorkoutPlansQuery, Result<PaginatedModel<WorkoutPlanModel>>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IWorkoutPlanRepository _workoutPlanRepository = workoutPlanRepository;

    public async Task<Result<PaginatedModel<WorkoutPlanModel>>> ExecuteAsync(GetMyWorkoutPlansQuery query, CancellationToken cancellationToken)
    {
        if (_userContext.Current.Role != UserRole.Client)
            return Result<PaginatedModel<WorkoutPlanModel>>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var plansQuery = _workoutPlanRepository.GetAllByUserIdAsQuery(_userContext.Current.UserId);
        if (!await plansQuery.AnyAsync(cancellationToken))
            return Result<PaginatedModel<WorkoutPlanModel>>.Fail(DomainErrors.NotFound(nameof(WorkoutPlan)));

        var paginated = await plansQuery.PaginateAsync(query.PaginationParams.PageNumber, query.PaginationParams.PageSize, cancellationToken);

        return Result<PaginatedModel<WorkoutPlanModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }
}
