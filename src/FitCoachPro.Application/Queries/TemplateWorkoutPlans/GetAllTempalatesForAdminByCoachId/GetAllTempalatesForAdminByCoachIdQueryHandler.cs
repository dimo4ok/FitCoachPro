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

namespace FitCoachPro.Application.Queries.TemplateWorkoutPlans.GetAllTempalatesForAdminByCoachId;

public class GetAllTempalatesForAdminByCoachIdQueryHandler(
    IUserContextService userContext,
    ITemplateWorkoutPlanRepository templateRepository,
    ILogger<GetAllTempalatesForAdminByCoachIdQueryHandler> logger
    ) : IQueryHandler<GetAllTempalatesForAdminByCoachIdQuery, Result<PaginatedModel<TemplateWorkoutPlanModel>>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly ITemplateWorkoutPlanRepository _templateRepository = templateRepository;
    private readonly ILogger<GetAllTempalatesForAdminByCoachIdQueryHandler> _logger = logger;

    public async Task<Result<PaginatedModel<TemplateWorkoutPlanModel>>> ExecuteAsync(GetAllTempalatesForAdminByCoachIdQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        _logger.LogInformation(
            "GetAllTemplatesForAdminByCoachId attempt started. AdminId: {AdminId}, CoachId: {CoachId}, Page: {PageNumber}, Size: {PageSize}",
            currentUser.UserId, query.CoachId, query.PaginationParams.PageNumber, query.PaginationParams.PageSize);

        if (currentUser.Role != UserRole.Admin)
        {
            _logger.LogWarning(
                "GetAllTemplatesForAdminByCoachId forbidden: User is not an Admin. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        var templatesQuery = _templateRepository.GetAllAsQuery(query.CoachId);
        if (!await templatesQuery.AnyAsync(cancellationToken))
        {
            _logger.LogWarning(
                "GetAllTemplatesForAdminByCoachId failed: No templates found. CoachId: {CoachId}",
                query.CoachId);
            return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Fail(DomainErrors.NotFound(nameof(TemplateWorkoutPlan)));
        }

        var paginated = await templatesQuery.PaginateAsync(query.PaginationParams.PageNumber, query.PaginationParams.PageSize, cancellationToken);

        _logger.LogInformation(
            "GetAllTemplatesForAdminByCoachId succeeded. CoachId: {CoachId}, ReturnedCount: {Count}, Total: {Total}",
            query.CoachId, paginated.Items.Count, paginated.TotalItems);

        return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }
}
