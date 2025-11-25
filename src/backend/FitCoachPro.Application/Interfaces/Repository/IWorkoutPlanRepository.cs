using FitCoachPro.Domain.Entities.Workouts.Plans;

namespace FitCoachPro.Application.Interfaces.Repository;

public interface IWorkoutPlanRepository
{
    IQueryable<WorkoutPlan> GetAllByUserIdAsQuery(Guid UserId);

    Task<WorkoutPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkoutPlan?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);

    Task CreateAsync(WorkoutPlan workoutPlan, CancellationToken cancellationToken = default);
    Task DeleteAsync(WorkoutPlan workoutPlan, CancellationToken cancellationToken = default);

    Task<bool> ExistsByClientAndDateAsync(Guid clientId, DateTime workoutDate, CancellationToken cancellationToken = default);
}