using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Models;
using FitCoachPro.Application.Common.Models.WorkoutItem;
using FitCoachPro.Application.Common.Models.WorkoutPlan;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repository;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Domain.Entities.Workouts.Items;

namespace FitCoachPro.Application.Services;

public class WorkoutPlanService(
    IUserContextService userContext,
    IWorkoutPlanRepository workoutPlanRepository, 
    IUserRepository userRepository, 
    IExerciseRepository exerciseRepository, 
    IUnitOfWork unitOfWork
        ) : IWorkoutPlanService
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IWorkoutPlanRepository _workoutPlanRepository = workoutPlanRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<WorkoutPlanModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var workoutPlan = await _workoutPlanRepository.GetByIdAsync(id, cancellationToken);
        if (workoutPlan == null)
            return Result<WorkoutPlanModel>.Fail(WorkoutPlanErrors.NotFound);

        if(!await HasUserAccessToWorkoutPlanAsync(_userContext.Current, workoutPlan.ClientId, cancellationToken))
            return Result<WorkoutPlanModel>.Fail(WorkoutPlanErrors.Forbidden, 403);

        return Result<WorkoutPlanModel>.Success(workoutPlan.ToModel());
    }

    //For Clients
    public async Task<Result<IReadOnlyList<WorkoutPlanModel>>> GetMyWorkoutPlansAsync(CancellationToken cancellationToken = default)
    {
        var workoutPlans = await _workoutPlanRepository.GetAllByUserIdAsync(_userContext.Current.UserId, cancellationToken);
        if (!workoutPlans.Any())
            return Result<IReadOnlyList<WorkoutPlanModel>>.Fail(WorkoutPlanErrors.NotFound);

        return Result<IReadOnlyList<WorkoutPlanModel>>.Success(workoutPlans.ToModel());
    }

    //For Coaches/Admins
    public async Task<Result<IReadOnlyList<WorkoutPlanModel>>> GetClientWorkoutPlansAsync(Guid clientId, CancellationToken cancellationToken = default)
    {
        if (!await HasUserAccessToWorkoutPlanAsync(_userContext.Current, clientId, cancellationToken))
            return Result<IReadOnlyList<WorkoutPlanModel>>.Fail(WorkoutPlanErrors.Forbidden, 403);

        var workoutPlans = await _workoutPlanRepository.GetAllByUserIdAsync(clientId, cancellationToken);
        if (!workoutPlans.Any())
            return Result<IReadOnlyList<WorkoutPlanModel>>.Fail(WorkoutPlanErrors.NotFound);

        return Result<IReadOnlyList<WorkoutPlanModel>>.Success(workoutPlans.ToModel());
    }

    public async Task<Result> CreateAsync(CreateWorkoutPlanModel model, CancellationToken cancellationToken = default)
    {
        if (!await HasUserAccessToWorkoutPlanAsync(_userContext.Current, model.ClientId, cancellationToken))
            return Result.Fail(WorkoutPlanErrors.Forbidden, 403);

        if (await _workoutPlanRepository.ExistsByClientAndDateAsync(model.ClientId, model.WorkoutDate, cancellationToken))
            return Result.Fail(WorkoutPlanErrors.AlreadyExists, 409);

        var exercises = await _exerciseRepository.GetAllAsync(cancellationToken);
        if (!exercises.Any())
            return Result.Fail(ExerciseErrors.NotFound);

        var (exercsieExistSuccess, exercsieExistError) = ExercisesExist(model.WorkoutItems, exercises);
        if(!exercsieExistSuccess)
            return Result.Fail(exercsieExistError!, 400);

        await _workoutPlanRepository.CreateAsync(model.ToEntity(), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(201);
    }

    public async Task<Result> UpdateAsync(Guid workoutPlanId, UpdateWorkoutPlanModel model, CancellationToken cancellationToken = default)
    {
        if (!await HasUserAccessToWorkoutPlanAsync(_userContext.Current, model.ClientId, cancellationToken))
            return Result.Fail(WorkoutPlanErrors.Forbidden, 403);

        var workoutPlan = await _workoutPlanRepository.GetByIdTrackedAsync(workoutPlanId, cancellationToken);
        if (workoutPlan == null)
            return Result.Fail(WorkoutPlanErrors.NotFound);

        if(workoutPlan.ClientId != model.ClientId)
            return Result.Fail(WorkoutPlanErrors.Forbidden, 403);

        if (await _workoutPlanRepository.ExistsByClientAndDateAsync(model.ClientId, model.WorkoutDate, cancellationToken)
            && model.WorkoutDate != workoutPlan.WorkoutDate)
            return Result.Fail(WorkoutPlanErrors.AlreadyExists, 409);

        workoutPlan.WorkoutDate = model.WorkoutDate;

        var exercises = await _exerciseRepository.GetAllAsync(cancellationToken);
        if(!exercises.Any())
            return Result.Fail(ExerciseErrors.NotFound);

        var (validateItemsSuccess, validateItemsError) = ValidateUpdateItems(workoutPlan.WorkoutItems, model.WorkoutItems, exercises);
        if (validateItemsSuccess == false)
            return Result.Fail(validateItemsError!, 400);

        SyncItems(workoutPlan.WorkoutItems, model.WorkoutItems);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var workoutPlan = await _workoutPlanRepository.GetByIdTrackedAsync(id, cancellationToken);
        if (workoutPlan == null)
            return Result.Fail(WorkoutPlanErrors.NotFound);

        if (!await HasUserAccessToWorkoutPlanAsync(_userContext.Current, workoutPlan.ClientId, cancellationToken))
            return Result.Fail(WorkoutPlanErrors.Forbidden, 403);

        await _workoutPlanRepository.DeleteAsync(workoutPlan, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(204);
    }

    private (bool, Error?) ExercisesExist(IEnumerable<CreateWorkoutItemModel> newItems, IReadOnlyList<Exercise> exercises)
    {
        var exercisIdsSet = exercises.Select(exercise => exercise.Id).ToHashSet();
        foreach (var ni in newItems)
        {
            if (!exercisIdsSet.Contains(ni.ExerciseId))
                return (false, ExerciseErrors.InvalidExerciseId);
        }

        return (true, null);
    }

    private (bool, Error?) ValidateUpdateItems(ICollection<WorkoutItem> currentItems, IEnumerable<UpdateWorkoutItemModel> newItems, IReadOnlyList<Exercise> exercises)
    {
        var exercisIdsSet = exercises.Select(exercise => exercise.Id).ToHashSet();

        foreach (var ni in newItems)
        {
            if (!exercisIdsSet.Contains(ni.ExerciseId))
                return (false, ExerciseErrors.InvalidExerciseId);

            if (ni.Id.HasValue && currentItems.All(ci => ci.Id != ni.Id))
                return (false, WorkoutItemErrors.InvalidWorkoutItemId);
        }

        return (true, null);
    }

    private void SyncItems(ICollection<WorkoutItem> currentItems, IEnumerable<UpdateWorkoutItemModel> newItems)
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
            if(currentItemForUpdate != null)
            {
                currentItemForUpdate.Description = ni.Description;
                currentItemForUpdate.ExerciseId = ni.ExerciseId;
                continue;
            }
        }
    }

    private async Task<bool> HasUserAccessToWorkoutPlanAsync(UserContext currentUser, Guid clientId, CancellationToken cancellationToken)
    {
        return currentUser.Role switch
        {
            UserRole.Client => clientId == currentUser.UserId,
            UserRole.Coach => await _userRepository.CanCoachAccessClientAsync(currentUser.UserId, clientId, cancellationToken),
            UserRole.Admin => true,
            _ => false
        };
    }
}