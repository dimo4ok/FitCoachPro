using FitCoachPro.Domain.Entities.Workouts;

namespace FitCoachPro.Application.Interfaces.Repository;

public interface IExerciseRepository
{
    IQueryable<Exercise> GetAllAsQuery();

    Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Exercise?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);

    Task CreateAsync(Exercise exercise, CancellationToken cancellationToken = default);
    void Delete(Exercise exercise);

    Task<bool> ExistsAsync(string exerciseName, CancellationToken cancellationToken = default);
}
