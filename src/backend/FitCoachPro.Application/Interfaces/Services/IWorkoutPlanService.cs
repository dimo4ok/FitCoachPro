using FitCoachPro.Application.Common.Models;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.WorkoutPlan;
using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Interfaces.Services
{
    public interface IWorkoutPlanService
    {
        Task<Result<WorkoutPlanModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Result<IReadOnlyList<WorkoutPlanModel>>> GetMyWorkoutPlansAsync(CancellationToken cancellationToken = default);
        Task<Result<IReadOnlyList<WorkoutPlanModel>>> GetClientWorkoutPlansAsync(Guid clientId, CancellationToken cancellationToken = default);

        Task<Result<PaginatedModel<WorkoutPlanModel>>> GetMyWorkoutPlansWithPaginationAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default);
        Task<Result<PaginatedModel<WorkoutPlanModel>>> GetClientWorkoutPlansWithPaginationAsync(Guid clientId, PaginationParams paginationParams, CancellationToken cancellationToken = default);


        Task<Result> CreateAsync(CreateWorkoutPlanModel model, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(Guid workoutPlanId, UpdateWorkoutPlanModel model, CancellationToken cancellationToken = default);
        Task<Result> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}