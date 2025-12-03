using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Extensions.WorkoutExtensions;
using FitCoachPro.Application.Common.Models.Exercise;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Application.Services;

public class ExerciseService(
    IUserContextService userContext,
    IExerciseRepository exerciseRepository,
    IUnitOfWork unitOfWork
    ) : IExerciseService
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IExerciseRepository _exerciseRepository = exerciseRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<ExerciseModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!HasUserAccess(_userContext.Current.Role))
            return Result<ExerciseModel>.Fail(ExerciseErrors.Forbidden, 403);

        var exercise = await _exerciseRepository.GetByIdAsync(id, cancellationToken);
        if (exercise == null)
            return Result<ExerciseModel>.Fail(ExerciseErrors.NotFound);

        return Result<ExerciseModel>.Success(exercise.ToModel());
    }

    public async Task<Result<PaginatedModel<ExerciseModel>>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        if (!HasUserAccess(_userContext.Current.Role))
            return Result<PaginatedModel<ExerciseModel>>.Fail(ExerciseErrors.Forbidden, 403);

        var query = _exerciseRepository.GetAllAsQuery();
        if (!await query.AnyAsync(cancellationToken))
            return Result<PaginatedModel<ExerciseModel>>.Fail(ExerciseErrors.NotFound);

        var paginated = await query.PaginateAsync(paginationParams.PageNumber, paginationParams.PageSize, cancellationToken);

        return Result<PaginatedModel<ExerciseModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }

    public async Task<Result> CreateAsync(CreateExerciseModel model, CancellationToken cancellationToken = default)
    {
        if (!HasUserAccess(_userContext.Current.Role))
            return Result.Fail(ExerciseErrors.Forbidden, 403);

        if (await _exerciseRepository.ExistsByExerciseNameAsync(model.ExerciseName.NormalizeValue(), cancellationToken))
            return Result.Fail(ExerciseErrors.AlreadyExists, 409);

        await _exerciseRepository.CreateAsync(model.ToEntity(), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(201);
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateExerciseModel model, CancellationToken cancellationToken = default)
    {
        if (!HasUserAccess(_userContext.Current.Role))
            return Result.Fail(ExerciseErrors.Forbidden, 403);

        var exercise = await _exerciseRepository.GetByIdAsync(id, cancellationToken);
        if (exercise == null)
            return Result.Fail(ExerciseErrors.NotFound);

        if (await _exerciseRepository.ExistsByExerciseNameForAnotherIdAsync(id, model.ExerciseName.NormalizeValue(), cancellationToken))
            return Result.Fail(ExerciseErrors.AlreadyExists, 409);

        if (!await CanModifyExerciseAsync(id, _userContext.Current.Role, cancellationToken))
            return Result.Fail(ExerciseErrors.UsedInActiveWorkoutPlan, 409);

        exercise.ExerciseName = model.ExerciseName;
        exercise.GifUrl = model.GifUrl;
        exercise.RowVersion = model.RowVersion;

        _exerciseRepository.Update(exercise);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id, DeleteExerciseModel model, CancellationToken cancellationToken = default)
    {
        if (!HasUserAccess(_userContext.Current.Role))
            return Result.Fail(ExerciseErrors.Forbidden, 403);

        var exercise = await _exerciseRepository.GetByIdAsync(id, cancellationToken);
        if (exercise == null)
            return Result.Fail(ExerciseErrors.NotFound);

        if (!await CanModifyExerciseAsync(id, _userContext.Current.Role, cancellationToken))
            return Result.Fail(ExerciseErrors.UsedInActiveWorkoutPlan, 409);

        exercise.RowVersion = model.RowVersion;

        _exerciseRepository.Delete(exercise);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(204);
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
