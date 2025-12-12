using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Extensions.WorkoutExtensions;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Workouts.TemplateWorkoutPlan;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Application.Queries.TemplateWorkoutPlans.GetAllTemplatesForCoach;

public class GetAllTemplatesForCoachQueryHandler(
    IUserContextService userContext,
    ITemplateWorkoutPlanRepository templateRepository
    ) : IQueryHandler<GetAllTemplatesForCoachQuery, Result<PaginatedModel<TemplateWorkoutPlanModel>>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly ITemplateWorkoutPlanRepository _templateRepository = templateRepository;

    public async Task<Result<PaginatedModel<TemplateWorkoutPlanModel>>> ExecuteAsync(GetAllTemplatesForCoachQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (_userContext.Current.Role != UserRole.Coach)
            return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var tempaltesQuery = _templateRepository.GetAllAsQuery(currentUser.UserId);
        if (!await tempaltesQuery.AnyAsync(cancellationToken))
            return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Fail(DomainErrors.NotFound(nameof(TemplateWorkoutPlan)));

        var paginated = await tempaltesQuery.PaginateAsync(query.PaginationParams.PageNumber, query.PaginationParams.PageSize, cancellationToken);
        return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }
}
