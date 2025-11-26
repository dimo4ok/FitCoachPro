using FitCoachPro.Application.Interfaces.Repository;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Infrastructure.Repositories;

public class ExerciseRepository(AppDbContext dbContext) : IExerciseRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public IQueryable<Exercise> GetAllAsQuery() =>
        _dbContext.Exercises
            .AsNoTracking()
            .OrderBy(x => x.ExerciseName);

    public async Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext.Exercises
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<Exercise?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext.Exercises.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task CreateAsync(Exercise exercise, CancellationToken cancellationToken = default) =>
        await _dbContext.Exercises.AddAsync(exercise, cancellationToken);

    public void Delete(Exercise exercise) =>
        _dbContext.Exercises.Remove(exercise);

    public async Task<bool> ExistsAsync(string exerciseName, CancellationToken cancellationToken = default) =>
        await _dbContext.Exercises.AnyAsync(
            x => x.ExerciseName.ToLower().Trim() == exerciseName.ToLower().Trim(),
            cancellationToken);
}
