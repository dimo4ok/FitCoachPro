using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using FitCoachPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Infrastructure.Repositories;

public class TemplateWorkoutPlanRepository(AppDbContext dbContext) : ITemplateWorkoutPlanRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public IQueryable<TemplateWorkoutPlan> GetAllAsQuery(Guid id) =>
       _dbContext.TemplateWorkoutPlans
            .AsNoTracking()
            .Where(x => x.CoachId == id)
            .OrderBy(x => x.UpdatedAt);

    public async Task<TemplateWorkoutPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool track = false)
    {
        var query = track
            ? _dbContext.TemplateWorkoutPlans
            : _dbContext.TemplateWorkoutPlans.AsNoTracking();

        return await query
            .Include(x => x.TemplateWorkoutItems)
            .ThenInclude(x => x.Exercise)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task CreateAsync(TemplateWorkoutPlan templatePlan, CancellationToken cancellationToken = default) =>
        await _dbContext.TemplateWorkoutPlans.AddAsync(templatePlan, cancellationToken);

    public void Delete(TemplateWorkoutPlan templatePlan) =>
        _dbContext.TemplateWorkoutPlans.Remove(templatePlan);

    public async Task<bool> ExistsByNameAndCoachIdAsync(string templateName, Guid coachId, CancellationToken cancellationToken = default) =>
        await _dbContext.TemplateWorkoutPlans
            .AnyAsync(x => x.TemplateName == templateName && x.CoachId == coachId, cancellationToken);
}
