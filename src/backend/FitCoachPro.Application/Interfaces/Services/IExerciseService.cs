using FitCoachPro.Application.Common.Models.Exercise;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Interfaces.Services;

public interface IExerciseService
{
    Task<Result<ExerciseDetailModel>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PaginatedModel<ExerciseDetailModel>>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default);

    Task<Result> CreateAsync(CreateExerciseModel model, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(Guid id, UpdateExerciseModel model, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(Guid id, DeleteExerciseModel model, CancellationToken cancellationToken = default);
}