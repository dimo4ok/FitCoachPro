using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions.WorkoutExtensions;
using FitCoachPro.Application.Common.Models.Workouts.TemplateWorkoutPlan;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using Microsoft.AspNetCore.Http;

namespace FitCoachPro.Application.Queries.TemplateWorkoutPlans.GetTemplateById;

public class GetTemplateByIdQueryHandler(
    IUserContextService userContext,
    ITemplateWorkoutPlanRepository templateRepository,
    ITemplateWorkoutPlanAccessService accessService
    ) : IQueryHandler<GetTemplateByIdQuery, Result<TemplateWorkoutPlanModel>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly ITemplateWorkoutPlanRepository _templateRepository = templateRepository;
    private readonly ITemplateWorkoutPlanAccessService _accessService = accessService;

    public async Task<Result<TemplateWorkoutPlanModel>> ExecuteAsync(GetTemplateByIdQuery query, CancellationToken cancellationToken)
    {
        var templatePlan = await _templateRepository.GetByIdAsync(query.Id, cancellationToken);
        if (templatePlan == null)
            return Result<TemplateWorkoutPlanModel>.Fail(DomainErrors.NotFound(nameof(TemplateWorkoutPlan)));

        if (!await _accessService.HasUserAccessToTemplateAsync(templatePlan.CoachId, _userContext.Current))
            return Result<TemplateWorkoutPlanModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        return Result<TemplateWorkoutPlanModel>.Success(templatePlan.ToModel());
    }
}
