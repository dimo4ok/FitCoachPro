using FitCoachPro.Application.Common.Models;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.WorkoutPlan;
using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Interfaces.Services
{
    public interface IWorkoutPlanService
    {
        Task<Result<WorkoutPlanModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Result<PaginatedModel<WorkoutPlanModel>>> GetMyWorkoutPlansAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default);
        Task<Result<PaginatedModel<WorkoutPlanModel>>> GetClientWorkoutPlansAsync(Guid clientId, PaginationParams paginationParams, CancellationToken cancellationToken = default);

        Task<Result> CreateAsync(CreateWorkoutPlanModel model, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(Guid workoutPlanId, UpdateWorkoutPlanModel model, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}