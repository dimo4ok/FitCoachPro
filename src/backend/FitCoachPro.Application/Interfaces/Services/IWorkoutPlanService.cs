using FitCoachPro.Application.Common.Models;
using FitCoachPro.Application.Common.Models.WorkoutPlan;
using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Interfaces.Services
{
    public interface IWorkoutPlanService
    {
        Task<Result<WorkoutPlanModel>> GetByIdAsync(Guid id, CurrentUserModel userModel, CancellationToken cancellationToken = default);

        Task<Result<IReadOnlyList<WorkoutPlanModel>>> GetMyWorkoutPlansAsync(CurrentUserModel userModel, CancellationToken cancellationToken = default);
        Task<Result<IReadOnlyList<WorkoutPlanModel>>> GetClientWorkoutPlansAsync(Guid clientId, CurrentUserModel userModel, CancellationToken cancellationToken = default);

        Task<Result> CreateAsync(CreateWorkoutPlanModel model, CurrentUserModel userModel, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(Guid workoutPlanId, UpdateWorkoutPlanModel model, CurrentUserModel userModel, CancellationToken cancellationToken = default);
        Task<Result> DeleteByIdAsync(Guid id, CurrentUserModel userModel, CancellationToken cancellationToken = default);
    }
}