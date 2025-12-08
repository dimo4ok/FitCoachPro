using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Extensions.WorkoutExtensions;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Workouts.Exercise;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Workouts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Application.Services.Workout;

public class ExerciseService(
    IUserContextService userContext,
    IExerciseRepository exerciseRepository,
    IUnitOfWork unitOfWork
    ) : IExerciseService
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<ExerciseDetailModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!HasUserAccess(_userContext.Current.Role))
            return Result<ExerciseDetailModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var exercise = await _exerciseRepository.GetByIdAsync(id, cancellationToken);
        if (exercise == null)
            return Result<ExerciseDetailModel>.Fail(DomainErrors.NotFound(nameof(Exercise)));

        return Result<ExerciseDetailModel>.Success(exercise.ToModel());
    }

    public async Task<Result<PaginatedModel<ExerciseDetailModel>>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        if (!HasUserAccess(_userContext.Current.Role))
            return Result<PaginatedModel<ExerciseDetailModel>>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var query = _exerciseRepository.GetAllAsQuery();
        if (!await query.AnyAsync(cancellationToken))
            return Result<PaginatedModel<ExerciseDetailModel>>.Fail(DomainErrors.NotFound(nameof(Exercise)));

        var paginated = await query.PaginateAsync(paginationParams.PageNumber, paginationParams.PageSize, cancellationToken);

        return Result<PaginatedModel<ExerciseDetailModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }

    public async Task<Result> CreateAsync(CreateExerciseModel model, CancellationToken cancellationToken = default)
    {
        if (!HasUserAccess(_userContext.Current.Role))
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        if (await _exerciseRepository.ExistsByExerciseNameAsync(model.ExerciseName.NormalizeValue(), cancellationToken))
            return Result.Fail(DomainErrors.AlreadyExists(nameof(Exercise)), StatusCodes.Status409Conflict);

        await _exerciseRepository.CreateAsync(model.ToEntity(), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(StatusCodes.Status201Created);
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateExerciseModel model, CancellationToken cancellationToken = default)
    {
        if (!HasUserAccess(_userContext.Current.Role))
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var exercise = await _exerciseRepository.GetByIdAsync(id, cancellationToken, track: true);
        if (exercise == null)
            return Result.Fail(DomainErrors.NotFound(nameof(Exercise)));

        if (await _exerciseRepository.ExistsByExerciseNameForAnotherIdAsync(id, model.ExerciseName.NormalizeValue(), cancellationToken))
            return Result.Fail(DomainErrors.AlreadyExists(nameof(Exercise)), StatusCodes.Status409Conflict);

        if (!await CanModifyExerciseAsync(id, _userContext.Current.Role, cancellationToken))
            return Result.Fail(DomainErrors.UsedInActiveEntity(nameof(Exercise)), StatusCodes.Status409Conflict);

        if (!exercise.RowVersion.SequenceEqual(model.RowVersion))
            return Result.Fail(SystemErrors.ConcurrencyConflict, StatusCodes.Status409Conflict);

        exercise.ExerciseName = model.ExerciseName;
        exercise.GifUrl = model.GifUrl;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, DeleteExerciseModel model, CancellationToken cancellationToken = default)
    {
        if (!HasUserAccess(_userContext.Current.Role))
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var exercise = await _exerciseRepository.GetByIdAsync(id, cancellationToken, track: true);
        if (exercise == null)
            return Result.Fail(DomainErrors.NotFound(nameof(Exercise)));

        if (!await CanModifyExerciseAsync(id, _userContext.Current.Role, cancellationToken))
            return Result.Fail(DomainErrors.UsedInActiveEntity(nameof(Exercise)), StatusCodes.Status409Conflict);

        if (!exercise.RowVersion.SequenceEqual(model.RowVersion))
            return Result.Fail(SystemErrors.ConcurrencyConflict, StatusCodes.Status409Conflict);

        _exerciseRepository.Delete(exercise);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(StatusCodes.Status204NoContent);
    }

    private bool HasUserAccess(UserRole currentRole) =>
        currentRole switch
        {
            UserRole.Coach => true,
            UserRole.Admin => true,
            _ => false
        };

    private async Task<bool> CanModifyExerciseAsync(Guid exerciseId, UserRole currentRole, CancellationToken cancellationToken) =>
        currentRole switch
        {
            UserRole.Coach => !await _exerciseRepository.IsExerciseUsedInActiveWorkoutPlanAsync(exerciseId, cancellationToken),
            UserRole.Admin => true,
            _ => false
        };
}
