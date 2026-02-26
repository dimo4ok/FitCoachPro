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
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Queries.TemplateWorkoutPlans.GetAllTemplatesForCoach;

public class GetAllTemplatesForCoachQueryHandler(
    IUserContextService userContext,
    ITemplateWorkoutPlanRepository templateRepository,
    ILogger<GetAllTemplatesForCoachQueryHandler> logger
    ) : IQueryHandler<GetAllTemplatesForCoachQuery, Result<PaginatedModel<TemplateWorkoutPlanModel>>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly ITemplateWorkoutPlanRepository _templateRepository = templateRepository;
    private readonly ILogger<GetAllTemplatesForCoachQueryHandler> _logger = logger;

    public async Task<Result<PaginatedModel<TemplateWorkoutPlanModel>>> ExecuteAsync(GetAllTemplatesForCoachQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Coach)
        {
            _logger.LogWarning(
                "GetAllTemplatesForCoach forbidden: User is not a Coach. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        _logger.LogInformation(
            "GetAllTemplatesForCoach attempt started. CoachId: {CoachId}, Page: {PageNumber}, Size: {PageSize}",
            currentUser.UserId, query.PaginationParams.PageNumber, query.PaginationParams.PageSize);

        var tempaltesQuery = _templateRepository.GetAllAsQuery(currentUser.UserId);
        if (!await tempaltesQuery.AnyAsync(cancellationToken))
        {
            _logger.LogWarning(
                "GetAllTemplatesForCoach failed: No templates found. CoachId: {CoachId}",
                currentUser.UserId);
            return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Fail(DomainErrors.NotFound(nameof(TemplateWorkoutPlan)));
        }

        var paginated = await tempaltesQuery.PaginateAsync(query.PaginationParams.PageNumber, query.PaginationParams.PageSize, cancellationToken);

        _logger.LogInformation(
            "GetAllTemplatesForCoach succeeded. CoachId: {CoachId}, ReturnedCount: {Count}, Total: {Total}",
            currentUser.UserId, paginated.Items.Count, paginated.TotalItems);

        return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }
}
