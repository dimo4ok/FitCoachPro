using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions.WorkoutExtensions;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Helpers;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using Microsoft.AspNetCore.Http;

namespace FitCoachPro.Application.Commands.TemplateWorkoutPlans.CreateTemplateWorkoutPlan;

public class CreateTemplateCommandHandler(
    IUserContextService userContext,
    ITemplateWorkoutPlanRepository templateRepository,
    IExerciseRepository exerciseRepository,
    IUnitOfWork unitOfWork,
    ITemplateWorkoutPlanHelper templateHelper
    ) : ICommandHandler<CreateTemplateCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly ITemplateWorkoutPlanRepository _templateRepository = templateRepository;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ITemplateWorkoutPlanHelper _templateHelper = templateHelper;

    public async Task<Result> ExecuteAsync(CreateTemplateCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        if (currentUser.Role != UserRole.Coach)
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        if (await _templateRepository.ExistsByNameAndCoachIdAsync(command.Model.TemplateName, currentUser.UserId, cancellationToken))
            return Result.Fail(DomainErrors.AlreadyExists(nameof(TemplateWorkoutPlan)), StatusCodes.Status409Conflict);

        var exerciseIdsSet = _exerciseRepository
            .GetAllAsQuery()
            .Select(exercise => exercise.Id)
            .ToHashSet();
        if (exerciseIdsSet.Count == 0)
            return Result.Fail(DomainErrors.NotFound(nameof(Exercise)));

        var (exerciseExistSuccess, exerciseExistError) = _templateHelper.ExercisesExist(command.Model.TemplateWorkoutItems, exerciseIdsSet);
        if (!exerciseExistSuccess)
            return Result.Fail(exerciseExistError!, StatusCodes.Status400BadRequest);

        var entity = command.Model.ToEntity(currentUser.UserId);

        await _templateRepository.CreateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(StatusCodes.Status201Created);
    }
}
