using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Workouts.TemplateWorkoutPlan;
using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Interfaces.Services;

public interface ITemplateWorkoutPlanService
{
    Task<Result<TemplateWorkoutPlanModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PaginatedModel<TemplateWorkoutPlanModel>>> GetAllForAdminByCoachIdAsync(Guid coachId, PaginationParams pagination, CancellationToken cancellationToken = default);
    Task<Result<PaginatedModel<TemplateWorkoutPlanModel>>> GetAllForCoachAsync(PaginationParams pagination, CancellationToken cancellationToken = default);

    Task<Result> CreateAsync(CreateTemplateWorkoutPlanModel model, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(Guid id, UpdateTemplateWorkoutPlanModel model, CancellationToken cancellationToken = default);
}