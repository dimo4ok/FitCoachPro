using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Extensions.WorkoutExtensions;
using FitCoachPro.Application.Common.Models;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.TemplateWorkoutItem;
using FitCoachPro.Application.Common.Models.TemplateWorkoutPlan;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repository;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Workouts.Items;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Application.Services;

public class TemplateWorkoutPlanService(
    IUserContextService userContext,
    ITemplateWorkoutPlanRepository templateRepository,
    IExerciseRepository exerciseRepository,
    IUnitOfWork unitOfWork
    ) : ITemplateWorkoutPlanService
{
    private readonly IUserContextService _userContext = userContext;
    private readonly ITemplateWorkoutPlanRepository _templateRepository = templateRepository;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<TemplateWorkoutPlanModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await HasUserAccessToTemplateAsync(id, _userContext.Current, cancellationToken))
            return Result<TemplateWorkoutPlanModel>.Fail(TemplateWorkoutPlanErrors.Forbidden, 403);

        var templatePlan = await _templateRepository.GetByIdAsync(id, cancellationToken);
        if (templatePlan == null)
            return Result<TemplateWorkoutPlanModel>.Fail(TemplateWorkoutPlanErrors.NotFound);

        return Result<TemplateWorkoutPlanModel>.Success(templatePlan.ToModel());
    }

    public async Task<Result<PaginatedModel<TemplateWorkoutPlanModel>>> GetAllForCoachAsync(PaginationParams pagination, CancellationToken cancellationToken = default)
    {
        var currentUser = _userContext.Current;

        if (_userContext.Current.Role != UserRole.Coach)
            return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Fail(TemplateWorkoutPlanErrors.Forbidden, 403);

        var query = _templateRepository.GetAllAsQuery(currentUser.UserId);
        if (!await query.AnyAsync(cancellationToken))
            return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Fail(TemplateWorkoutPlanErrors.NotFound);

        var paginated = await query.PaginateAsync(pagination.PageNumber, pagination.PageSize, cancellationToken);

        return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }

    public async Task<Result<PaginatedModel<TemplateWorkoutPlanModel>>> GetAllForAdminByCoachIdAsync(Guid coachId, PaginationParams pagination, CancellationToken cancellationToken = default)
    {
        if (_userContext.Current.Role != UserRole.Admin)
            return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Fail(TemplateWorkoutPlanErrors.Forbidden, 403);

        var query = _templateRepository.GetAllAsQuery(coachId);
        if (!await query.AnyAsync(cancellationToken))
            return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Fail(TemplateWorkoutPlanErrors.NotFound);

        var paginated = await query.PaginateAsync(pagination.PageNumber, pagination.PageSize, cancellationToken);

        return Result<PaginatedModel<TemplateWorkoutPlanModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }

    public async Task<Result> CreateAsync(CreateTemplateWorkoutPlanModel model, CancellationToken cancellationToken = default)
    {
        var currentUser = _userContext.Current;

        if (currentUser.Role != UserRole.Coach)
            return Result.Fail(TemplateWorkoutPlanErrors.Forbidden, 403);

        if (await _templateRepository.ExistsByNameAndCoachIdAsync(model.TemplateName, currentUser.UserId, cancellationToken))
            return Result.Fail(TemplateWorkoutPlanErrors.AlreadyExists, 409);

        var exerciseIdsSet = _exerciseRepository
            .GetAllAsQuery()
            .Select(exercise => exercise.Id)
            .ToHashSet();
        if (exerciseIdsSet.Count == 0)
            return Result.Fail(ExerciseErrors.NotFound);

        var (exerciseExistSuccess, exerciseExistError) = ExercisesExist(model.TemplateWorkoutItems, exerciseIdsSet);
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

        if (!await HasUserAccessToEditTemplateAsync(id, currentUser, cancellationToken))
            return Result.Fail(TemplateWorkoutPlanErrors.Forbidden, 403);

        var templatePlan = await _templateRepository.GetByIdTrackedAsync(id, cancellationToken);
        if (templatePlan == null)
            return Result.Fail(TemplateWorkoutPlanErrors.NotFound);

        if (await _templateRepository.ExistsByNameAndCoachIdAsync(model.TemplateName, currentUser.UserId, cancellationToken)
            && model.TemplateName != templatePlan.TemplateName)
            return Result.Fail(TemplateWorkoutPlanErrors.AlreadyExists, 409);

        var exerciseIdsSet = _exerciseRepository
          .GetAllAsQuery()
          .Select(exercise => exercise.Id)
          .ToHashSet();
        if (exerciseIdsSet.Count == 0)
            return Result.Fail(ExerciseErrors.NotFound);

        var (validateItemsSuccess, validateItemsError) = ValidateUpdateItems(templatePlan.TemplateWorkoutItems, model.TemplateWorkoutItems, exerciseIdsSet);
        if (validateItemsSuccess == false)
            return Result.Fail(validateItemsError!, 400);

        templatePlan.TemplateName = model.TemplateName;
        templatePlan.UpdatedAt = DateTime.UtcNow;

        SyncItems(templatePlan.TemplateWorkoutItems, model.TemplateWorkoutItems);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await HasUserAccessToEditTemplateAsync(id, _userContext.Current, cancellationToken))
            return Result.Fail(TemplateWorkoutPlanErrors.Forbidden, 403);

        var templatePlan = await _templateRepository.GetByIdTrackedAsync(id, cancellationToken);
        if (templatePlan == null)
            return Result.Fail(TemplateWorkoutPlanErrors.NotFound);

        _templateRepository.Delete(templatePlan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<bool> HasUserAccessToTemplateAsync(Guid templateId, UserContext currentUser, CancellationToken cancellationToken = default) =>
        currentUser.Role switch
        {
            UserRole.Admin => true,
            UserRole.Coach => await _templateRepository.ExistsByIdAndCoachIdAsync(templateId, currentUser.UserId, cancellationToken),
            _ => false
        };

    private async Task<bool> HasUserAccessToEditTemplateAsync(Guid templateId, UserContext currentUser, CancellationToken cancellationToken = default) =>
          currentUser.Role switch
          {
              UserRole.Coach => await _templateRepository.ExistsByIdAndCoachIdAsync(templateId, currentUser.UserId, cancellationToken),
              _ => false
          };

    private (bool, Error?) ExercisesExist(IEnumerable<CreateTemplateWorkoutItemModel> newItems, HashSet<Guid> exercisIdsSet)
    {
        foreach (var ni in newItems)
        {
            if (!exercisIdsSet.Contains(ni.ExerciseId))
                return (false, ExerciseErrors.InvalidExerciseId);
        }

        return (true, null);
    }

    private (bool, Error?) ValidateUpdateItems(ICollection<TemplateWorkoutItem> currentItems, IEnumerable<UpdateTemplateWorkoutItemModel> newItems, HashSet<Guid> exercisIdsSet)
    {
        foreach (var ni in newItems)
        {
            if (!exercisIdsSet.Contains(ni.ExerciseId))
                return (false, ExerciseErrors.InvalidExerciseId);

            if (ni.Id.HasValue && currentItems.All(ci => ci.Id != ni.Id))
                return (false, TemplateWorkoutItemErrors.InvalidTempalteWorkoutItemId);
        }

        return (true, null);
    }

    private void SyncItems(ICollection<TemplateWorkoutItem> currentItems, IEnumerable<UpdateTemplateWorkoutItemModel> newItems)
    {
        var itemsToRemove = currentItems.Where(ci => newItems.All(ni => ni.Id != ci.Id)).ToList();

        foreach (var i in itemsToRemove)
            currentItems.Remove(i);

        foreach (var ni in newItems)
        {
            if (!ni.Id.HasValue)
            {
                currentItems.Add(ni.ToEntity());
                continue;
            }

            var currentItemForUpdate = currentItems.FirstOrDefault(ci => ci.Id == ni.Id);
            if (currentItemForUpdate != null)
            {
                currentItemForUpdate.Description = ni.Description;
                currentItemForUpdate.ExerciseId = ni.ExerciseId;
                continue;
            }
        }
    }
}
