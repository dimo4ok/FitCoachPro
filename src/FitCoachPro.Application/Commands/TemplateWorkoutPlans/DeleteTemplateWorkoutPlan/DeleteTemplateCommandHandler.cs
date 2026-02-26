using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Commands.TemplateWorkoutPlans.DeleteTemplateWorkoutPlan;

public class DeleteTemplateCommandHandler(
    IUserContextService userContext,
    ITemplateWorkoutPlanRepository templateRepository,
    IUnitOfWork unitOfWork,
    ILogger<DeleteTemplateCommandHandler> logger
    ) : ICommandHandler<DeleteTemplateCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly ITemplateWorkoutPlanRepository _templateRepository = templateRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<DeleteTemplateCommandHandler> _logger = logger;

    public async Task<Result> ExecuteAsync(DeleteTemplateCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        _logger.LogInformation(
            "DeleteTemplateWorkoutPlan attempt started. TemplateId: {TemplateId}, CoachId: {CoachId}",
            command.Id, currentUser.UserId);

        var templatePlan = await _templateRepository.GetByIdAsync(command.Id, cancellationToken, track: true);
        if (templatePlan == null)
        {
            _logger.LogWarning(
                "DeleteTemplateWorkoutPlan failed: Template not found. TemplateId: {TemplateId}",
                command.Id);
            return Result.Fail(DomainErrors.NotFound(nameof(TemplateWorkoutPlan)));
        }

        if (currentUser.UserId != templatePlan.CoachId)
        {
            _logger.LogWarning(
                "DeleteTemplateWorkoutPlan forbidden. TemplateId: {TemplateId}, CoachId: {CoachId}, OwnerCoachId: {OwnerCoachId}",
                command.Id, currentUser.UserId, templatePlan.CoachId);
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        _templateRepository.Delete(templatePlan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "DeleteTemplateWorkoutPlan succeeded. TemplateId: {TemplateId}, CoachId: {CoachId}",
            command.Id, currentUser.UserId);

        return Result.Success(StatusCodes.Status204NoContent);
    }
}