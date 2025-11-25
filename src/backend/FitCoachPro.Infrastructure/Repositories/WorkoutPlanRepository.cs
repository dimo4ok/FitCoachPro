using FitCoachPro.Application.Interfaces.Repository;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using FitCoachPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Infrastructure.Repositories;

public class WorkoutPlanRepository(AppDbContext dbContext) : IWorkoutPlanRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public IQueryable<WorkoutPlan> GetAllByUserIdAsQuery(Guid UserId)
    {
        return _dbContext.WorkoutPlans
            .AsNoTracking()
            .Where(x => x.ClientId == UserId)
            .OrderBy(x => x.WorkoutDate).Reverse();
    }

    public async Task<WorkoutPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext.WorkoutPlans
            .AsNoTracking()
            .Include(x => x.WorkoutItems)
            .ThenInclude(x => x.Exercise)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<WorkoutPlan?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext.WorkoutPlans
            .Include(x => x.WorkoutItems)
            .ThenInclude(x => x.Exercise)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task CreateAsync(WorkoutPlan workoutPlan, CancellationToken cancellationToken = default)
        => await _dbContext.WorkoutPlans.AddAsync(workoutPlan, cancellationToken);

    public async Task DeleteAsync(WorkoutPlan workoutPlan, CancellationToken cancellationToken = default)
        => _dbContext.WorkoutPlans.Remove(workoutPlan);

    public async Task<bool> ExistsByClientAndDateAsync(Guid clientId, DateTime workoutDate, CancellationToken cancellationToken = default)
        => await _dbContext.WorkoutPlans.AnyAsync(
            x => x.ClientId == clientId &&
            x.WorkoutDate.Date == workoutDate.Date,
            cancellationToken);
}