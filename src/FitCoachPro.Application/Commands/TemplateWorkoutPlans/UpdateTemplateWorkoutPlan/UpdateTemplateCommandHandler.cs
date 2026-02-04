using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Helpers;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using Microsoft.AspNetCore.Http;

namespace FitCoachPro.Application.Commands.TemplateWorkoutPlans.UpdateTemplateWorkoutPlan;

public class UpdateTemplateCommandHandler(
    IUserContextService userContext,
    ITemplateWorkoutPlanRepository templateRepository,
    IExerciseRepository exerciseRepository,
    IUnitOfWork unitOfWork,
    ITemplateWorkoutPlanHelper templateHelper
    ) : ICommandHandler<UpdateTemplateCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly ITemplateWorkoutPlanRepository _templateRepository = templateRepository;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ITemplateWorkoutPlanHelper _templateHelper = templateHelper;

    public async Task<Result> ExecuteAsync(UpdateTemplateCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        var templatePlan = await _templateRepository.GetByIdAsync(command.Id, cancellationToken, track: true);
        if (templatePlan == null)
            return Result.Fail(DomainErrors.NotFound(nameof(TemplateWorkoutPlan)));

        if (currentUser.UserId != templatePlan.CoachId)
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        if (await _templateRepository.ExistsByNameAndCoachIdAsync(command.Model.TemplateName, currentUser.UserId, cancellationToken)
            && command.Model.TemplateName != templatePlan.TemplateName)
            return Result.Fail(DomainErrors.AlreadyExists(nameof(TemplateWorkoutPlan)), StatusCodes.Status409Conflict);

        var exerciseIdsSet = _exerciseRepository
          .GetAllAsQuery()
          .Select(exercise => exercise.Id)
          .ToHashSet();
        if (exerciseIdsSet.Count == 0)
            return Result.Fail(DomainErrors.NotFound(nameof(Exercise)));

        var (validateItemsSuccess, validateItemsError) = _templateHelper.ValidateUpdateItems(templatePlan.TemplateWorkoutItems, command.Model.TemplateWorkoutItems, exerciseIdsSet);
        if (validateItemsSuccess == false)
            return Result.Fail(validateItemsError!, StatusCodes.Status400BadRequest);

        templatePlan.TemplateName = command.Model.TemplateName;
        templatePlan.UpdatedAt = DateTime.UtcNow;

        _templateHelper.SyncItems(templatePlan.TemplateWorkoutItems, command.Model.TemplateWorkoutItems);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
