using FitCoachPro.Application.Interfaces.Repository;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using FitCoachPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Infrastructure.Repositories;

public class WorkoutPlanRepository : IWorkoutPlanRepository
{
    private readonly AppDbContext _dbContext;

    public WorkoutPlanRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<WorkoutPlan>> GetAllByUserIdAsync(Guid UserId, CancellationToken cancellationToken = default)
    {
        var workoutPlanList = await _dbContext.WorkoutPlans
            .AsNoTracking()
            .Where(x => x.ClientId == UserId)
            .Include(x => x.WorkoutItems)
            .ThenInclude(x => x.Exercise)
            .ToListAsync(cancellationToken);

        return workoutPlanList.AsReadOnly();
    }

    public async Task<WorkoutPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorkoutPlans
            .AsNoTracking()
            .Include(x => x.WorkoutItems)
            .ThenInclude(x => x.Exercise)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<WorkoutPlan?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorkoutPlans
            .Include(x => x.WorkoutItems)
            .ThenInclude(x => x.Exercise)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task CreateAsync(WorkoutPlan workoutPlan, CancellationToken cancellationToken = default)
    {
        await _dbContext.WorkoutPlans.AddAsync(workoutPlan, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(WorkoutPlan workoutPlan, CancellationToken cancellationToken = default)
    {
        _dbContext.WorkoutPlans.Remove(workoutPlan);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByClientAndDateAsync(Guid clientId, DateTime workoutDate, CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorkoutPlans.AnyAsync(
            x => x.ClientId == clientId &&
            x.WorkoutDate.Date == workoutDate.Date,
            cancellationToken);
    }
}