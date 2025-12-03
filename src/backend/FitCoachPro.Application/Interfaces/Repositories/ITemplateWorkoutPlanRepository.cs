using FitCoachPro.Domain.Entities.Workouts.Plans;

namespace FitCoachPro.Application.Interfaces.Repositories;

public interface ITemplateWorkoutPlanRepository
{
    IQueryable<TemplateWorkoutPlan> GetAllAsQuery(Guid id);
    Task<TemplateWorkoutPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool track = false);

    Task CreateAsync(TemplateWorkoutPlan plan, CancellationToken cancellationToken = default);
    void Delete(TemplateWorkoutPlan plan);

    Task<bool> ExistsByNameAndCoachIdAsync(string templateName, Guid coachId, CancellationToken cancellationToken = default);
}