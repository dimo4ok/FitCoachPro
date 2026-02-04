using FitCoachPro.Domain.Entities.Workouts;

namespace FitCoachPro.Application.Interfaces.Repositories;

public interface IExerciseRepository
{
    IQueryable<Exercise> GetAllAsQuery();

    Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool track = false);

    Task CreateAsync(Exercise exercise, CancellationToken cancellationToken = default);
    void Delete(Exercise exercise);

    Task<bool> ExistsByExerciseNameAsync(string exerciseName, CancellationToken cancellationToken = default);
    Task<bool> ExistsByExerciseNameForAnotherIdAsync(Guid id, string exerciseName, CancellationToken cancellationToken = default);
    Task<bool> IsExerciseUsedInActiveWorkoutPlanAsync(Guid id, CancellationToken cancellationToken = default);
}
