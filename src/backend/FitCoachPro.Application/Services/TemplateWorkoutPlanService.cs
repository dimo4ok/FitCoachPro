using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Extensions.WorkoutExtensions;
using FitCoachPro.Application.Common.Models;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.TemplateWorkoutPlan;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Helpers;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Application.Services;

public class TemplateWorkoutPlanService(
    IUserContextService userContext,
    ITemplateWorkoutPlanRepository templateRepository,
    IExerciseRepository exerciseRepository,
    IUnitOfWork unitOfWork,
    ITemplateWorkoutPlanHelper templateHelper
    ) : ITemplateWorkoutPlanService
{
    private readonly IUserContextService _userContext = userContext;
    private readonly ITemplateWorkoutPlanRepository _templateRepository = templateRepository;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ITemplateWorkoutPlanHelper _templateHelper = templateHelper;

    public async Task<Result<TemplateWorkoutPlanModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var templatePlan = await _templateRepository.GetByIdAsync(id, cancellationToken);
        if (templatePlan == null)
            return Result<TemplateWorkoutPlanModel>.Fail(DomainErrors.NotFound(nameof(TemplateWorkoutPlan)));

        if (!await HasUserAccessToTemplateAsync(templatePlan.CoachId, _userContext.Current))
            return Result<TemplateWorkoutPlanModel>.Fail(DomainErrors.Forbidden, 403);

        return Result<TemplateWorkoutPlanModel>.Success(templatePlan.ToModel());
    }

    public async Task<Result<PaginatedModel<TemplateWorkoutPlanModel>>> GetAllForCoachAsync(PaginationParams pagination, CancellationToken cancellationToken = default)
    {
        var currentUser = _userContext.Current;

        if (_userContext.Current.Role != UserRole.Coach)
            return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Fail(DomainErrors.Forbidden, 403);

        var query = _templateRepository.GetAllAsQuery(currentUser.UserId);
        if (!await query.AnyAsync(cancellationToken))
            return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Fail(DomainErrors.NotFound(nameof(TemplateWorkoutPlan)));

        var paginated = await query.PaginateAsync(pagination.PageNumber, pagination.PageSize, cancellationToken);

        return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }

    public async Task<Result<PaginatedModel<TemplateWorkoutPlanModel>>> GetAllForAdminByCoachIdAsync(Guid coachId, PaginationParams pagination, CancellationToken cancellationToken = default)
    {
        if (_userContext.Current.Role != UserRole.Admin)
            return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Fail(DomainErrors.Forbidden, 403);

        var query = _templateRepository.GetAllAsQuery(coachId);
        if (!await query.AnyAsync(cancellationToken))
            return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Fail(DomainErrors.NotFound(nameof(TemplateWorkoutPlan)));

        var paginated = await query.PaginateAsync(pagination.PageNumber, pagination.PageSize, cancellationToken);

        return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }

    public async Task<Result> CreateAsync(CreateTemplateWorkoutPlanModel model, CancellationToken cancellationToken = default)
    {
        var currentUser = _userContext.Current;

        if (currentUser.Role != UserRole.Coach)
            return Result.Fail(DomainErrors.Forbidden, 403);

        if (await _templateRepository.ExistsByNameAndCoachIdAsync(model.TemplateName, currentUser.UserId, cancellationToken))
            return Result.Fail(DomainErrors.AlreadyExists(nameof(TemplateWorkoutPlan)), 409);

        var exerciseIdsSet = _exerciseRepository
            .GetAllAsQuery()
            .Select(exercise => exercise.Id)
            .ToHashSet();
        if (exerciseIdsSet.Count == 0)
            return Result.Fail(DomainErrors.NotFound(nameof(Exercise)));

        var (exerciseExistSuccess, exerciseExistError) = _templateHelper.ExercisesExist(model.TemplateWorkoutItems, exerciseIdsSet);
        if (!exerciseExistSuccess)
            return Result.Fail(exerciseExistError!, 400);

        var entity = model.ToEntity(currentUser.UserId);

        await _templateRepository.CreateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(201);
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateTemplateWorkoutPlanModel model, CancellationToken cancellationToken = default)
    {
        var currentUser = _userContext.Current;

        var templatePlan = await _templateRepository.GetByIdAsync(id, cancellationToken, track: true);
        if (templatePlan == null)
            return Result.Fail(DomainErrors.NotFound(nameof(TemplateWorkoutPlan)));

        if (currentUser.UserId != templatePlan.CoachId)
            return Result.Fail(DomainErrors.Forbidden, 403);

        if (await _templateRepository.ExistsByNameAndCoachIdAsync(model.TemplateName, currentUser.UserId, cancellationToken)
            && model.TemplateName != templatePlan.TemplateName)
            return Result.Fail(DomainErrors.AlreadyExists(nameof(TemplateWorkoutPlan)), 409);

        var exerciseIdsSet = _exerciseRepository
          .GetAllAsQuery()
          .Select(exercise => exercise.Id)
          .ToHashSet();
        if (exerciseIdsSet.Count == 0)
            return Result.Fail(DomainErrors.NotFound(nameof(Exercise)));

        var (validateItemsSuccess, validateItemsError) = _templateHelper.ValidateUpdateItems(templatePlan.TemplateWorkoutItems, model.TemplateWorkoutItems, exerciseIdsSet);
        if (validateItemsSuccess == false)
            return Result.Fail(validateItemsError!, 400);

        templatePlan.TemplateName = model.TemplateName;
        templatePlan.UpdatedAt = DateTime.UtcNow;

        _templateHelper.SyncItems(templatePlan.TemplateWorkoutItems, model.TemplateWorkoutItems);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var templatePlan = await _templateRepository.GetByIdAsync(id, cancellationToken, track: true);
        if (templatePlan == null)
            return Result.Fail(DomainErrors.NotFound(nameof(TemplateWorkoutPlan)));

        if (_userContext.Current.UserId != templatePlan.CoachId)
            return Result.Fail(DomainErrors.Forbidden, 403);

        _templateRepository.Delete(templatePlan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<bool> HasUserAccessToTemplateAsync(Guid tempalteCoachId, UserContext currentUser) =>
        currentUser.Role switch
        {
            UserRole.Admin => true,
            UserRole.Coach => currentUser.UserId == tempalteCoachId,
            _ => false
        };
}
