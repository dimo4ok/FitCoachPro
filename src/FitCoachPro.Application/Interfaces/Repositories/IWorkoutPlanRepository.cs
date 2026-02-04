using FitCoachPro.Domain.Entities.Workouts.Plans;

namespace FitCoachPro.Application.Interfaces.Repositories;

public interface IWorkoutPlanRepository
{
    IQueryable<WorkoutPlan> GetAllByUserIdAsQuery(Guid UserId);
    Task<WorkoutPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool track = false);

    Task CreateAsync(WorkoutPlan workoutPlan, CancellationToken cancellationToken = default);
    void Delete(WorkoutPlan workoutPlan);

    Task<bool> ExistsByClientAndDateAsync(Guid clientId, DateTime workoutDate, CancellationToken cancellationToken = default);
}