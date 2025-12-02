using FitCoachPro.Domain.Entities.Workouts;

namespace FitCoachPro.Application.Interfaces.Repository;

public interface IExerciseRepository
{
    IQueryable<Exercise> GetAllAsQuery();

    Task<Exercise?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task CreateAsync(Exercise exercise, CancellationToken cancellationToken = default);
    void Update(Exercise exercise);
    void Delete(Exercise exercise);

    Task<bool> ExistsByExerciseNameAsync(string exerciseName, CancellationToken cancellationToken = default);
    Task<bool> ExistsByExerciseNameForAnotherIdAsync(Guid id, string exerciseName, CancellationToken cancellationToken = default);
    Task<bool> IsExerciseUsedInActiveWorkoutPlanAsync(Guid id, CancellationToken cancellationToken = default);
}
