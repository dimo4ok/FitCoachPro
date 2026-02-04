using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Infrastructure.Repositories.WorkoutRepositories;

public class ExerciseRepository(AppDbContext dbContext) : IExerciseRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public IQueryable<Exercise> GetAllAsQuery() =>
        _dbContext.Exercises
            .AsNoTracking()
            .OrderBy(x => x.ExerciseName);

    public async Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool track = false)
    {
        var query = track
            ? _dbContext.Exercises
            : _dbContext.Exercises.AsNoTracking();

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task CreateAsync(Exercise exercise, CancellationToken cancellationToken = default) =>
        await _dbContext.Exercises.AddAsync(exercise, cancellationToken);

    public void Delete(Exercise exercise) =>
        _dbContext.Exercises.Remove(exercise);

    public async Task<bool> ExistsByExerciseNameAsync(string normalizedExerciseName, CancellationToken cancellationToken = default) =>
        await _dbContext.Exercises
            .AnyAsync(x =>
                x.ExerciseName.ToLower().Trim() == normalizedExerciseName,
                cancellationToken);

    public async Task<bool> ExistsByExerciseNameForAnotherIdAsync(Guid id, string normalizedExerciseName, CancellationToken cancellationToken = default) =>
        await _dbContext.Exercises
            .AnyAsync(x =>
                x.Id != id &&
                x.ExerciseName.ToLower().Trim() == normalizedExerciseName,
                cancellationToken);

    public async Task<bool> IsExerciseUsedInActiveWorkoutPlanAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext.WorkoutPlans
            .AnyAsync(x =>
                x.WorkoutItems.Any(i => i.ExerciseId == id) &&
                x.WorkoutDate.Date.AddDays(2) >= DateTime.UtcNow.Date,
                cancellationToken);
}
