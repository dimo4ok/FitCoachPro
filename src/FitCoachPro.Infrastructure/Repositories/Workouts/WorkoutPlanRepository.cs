using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using FitCoachPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Infrastructure.Repositories.WorkoutRepositories;

public class WorkoutPlanRepository(AppDbContext dbContext) : IWorkoutPlanRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public IQueryable<WorkoutPlan> GetAllByUserIdAsQuery(Guid UserId) =>
        _dbContext.WorkoutPlans
            .AsNoTracking()
            .Where(x => x.ClientId == UserId)
            .OrderBy(x => x.WorkoutDate).Reverse();

    public async Task<WorkoutPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool track = false)
    {
        var query = track 
            ? _dbContext.WorkoutPlans
            : _dbContext.WorkoutPlans.AsNoTracking();

        return await query
            .Include(x => x.WorkoutItems)
            .ThenInclude(x => x.Exercise)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
        
    public async Task CreateAsync(WorkoutPlan workoutPlan, CancellationToken cancellationToken = default) =>
        await _dbContext.WorkoutPlans.AddAsync(workoutPlan, cancellationToken);

    public void Delete(WorkoutPlan workoutPlan) => 
        _dbContext.WorkoutPlans.Remove(workoutPlan);

    public async Task<bool> ExistsByClientAndDateAsync(Guid clientId, DateTime workoutDate, CancellationToken cancellationToken = default) =>
        await _dbContext.WorkoutPlans
            .AnyAsync(x => 
                x.ClientId == clientId &&
                x.WorkoutDate.Date == workoutDate.Date,
                cancellationToken);
}