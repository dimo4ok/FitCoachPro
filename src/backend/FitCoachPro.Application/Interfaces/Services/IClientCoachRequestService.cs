using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Requests;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Interfaces.Services;

public interface IClientCoachRequestService
{
    Task<Result<ClientCoachRequestModel>> GetByIdAsync(Guid requestId, CancellationToken cancellationToken = default);
    Task<Result<PaginatedModel<ClientCoachRequestModel>>> GetAllForCoachOrClientAsync(PaginationParams paginationParams, CoachRequestStatus? status = null, CancellationToken cancellationToken = default);
    Task<Result<PaginatedModel<ClientCoachRequestModel>>> GetAllForAdminAsync(Guid userId, PaginationParams paginationParams, CoachRequestStatus? status = null, CancellationToken cancellationToken = default);

    Task<Result> CreateAsync(Guid coachId, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(Guid requstId, UpdateClientCoachRequestModel model, CancellationToken cancellationToken = default);
    Task<Result> CancelRequestAsync(Guid requestId, DeleteClientCoachRequestModel model, CancellationToken cancellationToken = default);
    Task<Result> DeleteOwnRequestsAsync(CancellationToken cancellationToken = default);
}