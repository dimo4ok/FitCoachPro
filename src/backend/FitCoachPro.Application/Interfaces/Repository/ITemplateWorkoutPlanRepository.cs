using FitCoachPro.Domain.Entities.Workouts.Plans;

namespace FitCoachPro.Application.Interfaces.Repository;

public interface ITemplateWorkoutPlanRepository
{
    IQueryable<TemplateWorkoutPlan> GetAllAsQuery(Guid id);

    Task<TemplateWorkoutPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TemplateWorkoutPlan?> GetByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);

    Task CreateAsync(TemplateWorkoutPlan plan, CancellationToken cancellationToken = default);
    void Delete(TemplateWorkoutPlan plan);

    Task<bool> ExistsByIdAndCoachIdAsync(Guid templateId, Guid coachId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAndCoachIdAsync(string templateName, Guid coachId, CancellationToken cancellationToken = default);
}