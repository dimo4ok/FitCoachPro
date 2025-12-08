using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Extensions.WorkoutExtensions;
using FitCoachPro.Application.Common.Models;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Workouts.WorkoutPlan;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Helpers;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Application.Services.Workout;

public class WorkoutPlanService(
    IUserContextService userContext,
    IWorkoutPlanRepository workoutPlanRepository,
    IUserRepository userRepository,
    IExerciseRepository exerciseRepository,
    IUnitOfWork unitOfWork,
    IWorkoutPlanHelper workoutPlanHelper
        ) : IWorkoutPlanService
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IWorkoutPlanRepository _workoutPlanRepository = workoutPlanRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IWorkoutPlanHelper _workoutPlanHelper = workoutPlanHelper;

    public async Task<Result<WorkoutPlanModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var workoutPlan = await _workoutPlanRepository.GetByIdAsync(id, cancellationToken);
        if (workoutPlan == null)
            return Result<WorkoutPlanModel>.Fail(DomainErrors.NotFound(nameof(WorkoutPlan)));

        if (!await HasUserAccessToWorkoutPlanAsync(_userContext.Current, workoutPlan.ClientId, cancellationToken))
            return Result<WorkoutPlanModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        return Result<WorkoutPlanModel>.Success(workoutPlan.ToModel());
    }

    public async Task<Result<PaginatedModel<WorkoutPlanModel>>> GetMyWorkoutPlansAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        if (_userContext.Current.Role != UserRole.Client)
            return Result<PaginatedModel<WorkoutPlanModel>>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var query = _workoutPlanRepository.GetAllByUserIdAsQuery(_userContext.Current.UserId);
        if (!await query.AnyAsync(cancellationToken))
            return Result<PaginatedModel<WorkoutPlanModel>>.Fail(DomainErrors.NotFound(nameof(WorkoutPlan)));

        var paginated = await query.PaginateAsync(paginationParams.PageNumber, paginationParams.PageSize, cancellationToken);

        return Result<PaginatedModel<WorkoutPlanModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }

    public async Task<Result<PaginatedModel<WorkoutPlanModel>>> GetClientWorkoutPlansAsync(Guid clientId, PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        var currentUser = _userContext.Current;

        if (currentUser.Role != UserRole.Admin && !await HasCoachAccessToWorkoutPlan(currentUser, clientId, cancellationToken))
            return Result<PaginatedModel<WorkoutPlanModel>>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var query = _workoutPlanRepository.GetAllByUserIdAsQuery(clientId);
        if (!await query.AnyAsync(cancellationToken))
            return Result<PaginatedModel<WorkoutPlanModel>>.Fail(DomainErrors.NotFound(nameof(WorkoutPlan)));

        var paginated = await query.PaginateAsync(paginationParams.PageNumber, paginationParams.PageSize, cancellationToken);

        return Result<PaginatedModel<WorkoutPlanModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }

    public async Task<Result> CreateAsync(CreateWorkoutPlanModel model, CancellationToken cancellationToken = default)
    {
        if (!await HasCoachAccessToWorkoutPlan(_userContext.Current, model.ClientId, cancellationToken))
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        if (await _workoutPlanRepository.ExistsByClientAndDateAsync(model.ClientId, model.WorkoutDate, cancellationToken))
            return Result.Fail(DomainErrors.AlreadyExists(nameof(WorkoutPlan)), StatusCodes.Status409Conflict);

        var exerciseIdsSet = _exerciseRepository
           .GetAllAsQuery()
           .Select(exercise => exercise.Id)
           .ToHashSet();
        if (exerciseIdsSet.Count == 0)
            return Result.Fail(DomainErrors.NotFound(nameof(Exercise)));

        var (exercsieExistSuccess, exercsieExistError) = _workoutPlanHelper.ExercisesExist(model.WorkoutItems, exerciseIdsSet);
        if (!exercsieExistSuccess)
            return Result.Fail(exercsieExistError!, StatusCodes.Status400BadRequest);

        await _workoutPlanRepository.CreateAsync(model.ToEntity(), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(StatusCodes.Status201Created);
    }

    public async Task<Result> UpdateAsync(Guid workoutPlanId, UpdateWorkoutPlanModel model, CancellationToken cancellationToken = default)
    {
        var workoutPlan = await _workoutPlanRepository.GetByIdAsync(workoutPlanId, cancellationToken, track: true);
        if (workoutPlan == null)
            return Result.Fail(DomainErrors.NotFound(nameof(WorkoutPlan)));

        if (!await HasCoachAccessToWorkoutPlan(_userContext.Current, workoutPlan.ClientId, cancellationToken))
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        if (await _workoutPlanRepository.ExistsByClientAndDateAsync(workoutPlan.ClientId, model.WorkoutDate, cancellationToken)
            && model.WorkoutDate != workoutPlan.WorkoutDate)
            return Result.Fail(DomainErrors.AlreadyExists(nameof(WorkoutPlan)), StatusCodes.Status409Conflict);

        workoutPlan.WorkoutDate = model.WorkoutDate;

        var exerciseIdsSet = _exerciseRepository
           .GetAllAsQuery()
           .Select(exercise => exercise.Id)
           .ToHashSet();
        if (exerciseIdsSet.Count == 0)
            return Result.Fail(DomainErrors.NotFound(nameof(Exercise)));

        var (validateItemsSuccess, validateItemsError) = _workoutPlanHelper.ValidateUpdateItems(workoutPlan.WorkoutItems, model.WorkoutItems, exerciseIdsSet);
        if (validateItemsSuccess == false)
            return Result.Fail(validateItemsError!, StatusCodes.Status400BadRequest);

        _workoutPlanHelper.SyncItems(workoutPlan.WorkoutItems, model.WorkoutItems);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var workoutPlan = await _workoutPlanRepository.GetByIdAsync(id, cancellationToken, track: true);
        if (workoutPlan == null)
            return Result.Fail(DomainErrors.NotFound(nameof(WorkoutPlan)));

        if (!await HasCoachAccessToWorkoutPlan(_userContext.Current, workoutPlan.ClientId, cancellationToken))
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        _workoutPlanRepository.Delete(workoutPlan);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(StatusCodes.Status204NoContent);
    }

    private async Task<bool> HasUserAccessToWorkoutPlanAsync(UserContext currentUser, Guid clientId, CancellationToken cancellationToken) =>
        currentUser.Role switch
        {
            UserRole.Client => clientId == currentUser.UserId,
            UserRole.Coach => await _userRepository.CanCoachAccessClientAsync(currentUser.UserId, clientId, cancellationToken),
            UserRole.Admin => true,
            _ => false
        };

    private async Task<bool> HasCoachAccessToWorkoutPlan(UserContext currentUser, Guid clientId, CancellationToken cancellationToken) =>
        currentUser.Role switch
        {
            UserRole.Coach => await _userRepository.CanCoachAccessClientAsync(currentUser.UserId, clientId, cancellationToken),
            _ => false
        };
}