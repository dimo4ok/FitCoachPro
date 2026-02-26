using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Helpers;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Commands.TemplateWorkoutPlans.UpdateTemplateWorkoutPlan;

public class UpdateTemplateCommandHandler(
    IUserContextService userContext,
    ITemplateWorkoutPlanRepository templateRepository,
    IExerciseRepository exerciseRepository,
    IUnitOfWork unitOfWork,
    ITemplateWorkoutPlanHelper templateHelper,
    ILogger<UpdateTemplateCommandHandler> logger
    ) : ICommandHandler<UpdateTemplateCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly ITemplateWorkoutPlanRepository _templateRepository = templateRepository;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ITemplateWorkoutPlanHelper _templateHelper = templateHelper;
    private readonly ILogger<UpdateTemplateCommandHandler> _logger = logger;

    public async Task<Result> ExecuteAsync(UpdateTemplateCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        _logger.LogInformation(
            "UpdateTemplateWorkoutPlan attempt started. TemplateId: {TemplateId}, CoachId: {CoachId}, NewName: {TemplateName}",
            command.Id, currentUser.UserId, command.Model.TemplateName);

        var templatePlan = await _templateRepository.GetByIdAsync(command.Id, cancellationToken, track: true);
        if (templatePlan == null)
        {
            _logger.LogWarning(
                "UpdateTemplateWorkoutPlan failed: Template not found. TemplateId: {TemplateId}",
                command.Id);
            return Result.Fail(DomainErrors.NotFound(nameof(TemplateWorkoutPlan)));
        }

        if (currentUser.UserId != templatePlan.CoachId)
        {
            _logger.LogWarning(
                "UpdateTemplateWorkoutPlan forbidden. TemplateId: {TemplateId}, CoachId: {CoachId}, OwnerCoachId: {OwnerCoachId}",
                command.Id, currentUser.UserId, templatePlan.CoachId);
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        if (await _templateRepository.ExistsByNameAndCoachIdAsync(command.Model.TemplateName, currentUser.UserId, cancellationToken)
            && command.Model.TemplateName != templatePlan.TemplateName)
        {
            _logger.LogWarning(
                "UpdateTemplateWorkoutPlan failed: Template with name already exists. CoachId: {CoachId}, TemplateName: {TemplateName}",
                currentUser.UserId, command.Model.TemplateName);
            return Result.Fail(DomainErrors.AlreadyExists(nameof(TemplateWorkoutPlan)), StatusCodes.Status409Conflict);
        }

        var exerciseIdsSet = _exerciseRepository
          .GetAllAsQuery()
          .Select(exercise => exercise.Id)
          .ToHashSet();
        if (exerciseIdsSet.Count == 0)
        {
            _logger.LogWarning(
                "UpdateTemplateWorkoutPlan failed: No exercises found in system. TemplateId: {TemplateId}",
                command.Id);
            return Result.Fail(DomainErrors.NotFound(nameof(Exercise)));
        }

        var (validateItemsSuccess, validateItemsError) = _templateHelper.ValidateUpdateItems(templatePlan.TemplateWorkoutItems, command.Model.TemplateWorkoutItems, exerciseIdsSet);
        if (validateItemsSuccess == false)
        {
            _logger.LogWarning(
                "UpdateTemplateWorkoutPlan failed: Invalid items update payload. TemplateId: {TemplateId}. Error: {@Error}",
                command.Id, validateItemsError);
            return Result.Fail(validateItemsError!, StatusCodes.Status400BadRequest);
        }

        templatePlan.TemplateName = command.Model.TemplateName;
        templatePlan.UpdatedAt = DateTime.UtcNow;

        _templateHelper.SyncItems(templatePlan.TemplateWorkoutItems, command.Model.TemplateWorkoutItems);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "UpdateTemplateWorkoutPlan succeeded. TemplateId: {TemplateId}, CoachId: {CoachId}, TemplateName: {TemplateName}",
            templatePlan.Id, currentUser.UserId, templatePlan.TemplateName);

        return Result.Success();
    }
}
