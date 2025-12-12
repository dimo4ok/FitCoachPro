using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using Microsoft.AspNetCore.Http;

namespace FitCoachPro.Application.Commands.TemplateWorkoutPlans.DeleteTemplateWorkoutPlan;

public class DeleteTemplateCommandHandler(
    IUserContextService userContext,
    ITemplateWorkoutPlanRepository templateRepository,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<DeleteTemplateCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly ITemplateWorkoutPlanRepository _templateRepository = templateRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> ExecuteAsync(DeleteTemplateCommand command, CancellationToken cancellationToken)
    {
        var templatePlan = await _templateRepository.GetByIdAsync(command.Id, cancellationToken, track: true);
        if (templatePlan == null)
            return Result.Fail(DomainErrors.NotFound(nameof(TemplateWorkoutPlan)));

        if (_userContext.Current.UserId != templatePlan.CoachId)
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        _templateRepository.Delete(templatePlan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(StatusCodes.Status204NoContent);
    }
}