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

namespace FitCoachPro.Application.Queries.TemplateWorkoutPlans.GetAllTempalatesForAdminByCoachId;

public class GetAllTempalatesForAdminByCoachIdQueryHandler(
    IUserContextService userContext,
    ITemplateWorkoutPlanRepository templateRepository
    ) : IQueryHandler<GetAllTempalatesForAdminByCoachIdQuery, Result<PaginatedModel<TemplateWorkoutPlanModel>>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly ITemplateWorkoutPlanRepository _templateRepository = templateRepository;

    public async Task<Result<PaginatedModel<TemplateWorkoutPlanModel>>> ExecuteAsync(GetAllTempalatesForAdminByCoachIdQuery query, CancellationToken cancellationToken)
    {
        if (_userContext.Current.Role != UserRole.Admin)
            return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var templatesQuery = _templateRepository.GetAllAsQuery(query.CoachId);
        if (!await templatesQuery.AnyAsync(cancellationToken))
            return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Fail(DomainErrors.NotFound(nameof(TemplateWorkoutPlan)));

        var paginated = await templatesQuery.PaginateAsync(query.PaginationParams.PageNumber, query.PaginationParams.PageSize, cancellationToken);

        return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }
}
