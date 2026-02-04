using FitCoachPro.Domain.Entities;
using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Interfaces.Repositories;

public interface IClientCoachRequestRepository
{
    Task<ClientCoachRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool track = false);
    IQueryable<ClientCoachRequest> GetAllByUserIdAsQuery(Guid userId);
    IQueryable < ClientCoachRequest> GetAllByUserIdAndUserRoleAsQuery(Guid userId, UserRole role, bool track = false);

    Task CreateAsync(ClientCoachRequest request, CancellationToken cancellationToken = default);
    void Delete(ClientCoachRequest request);
    void DeleteRequests(IEnumerable<ClientCoachRequest> requests);

    Task<bool> IsDuplicateRequestAsync(Guid clientId, Guid coachId, CancellationToken cancellationToken = default);
    Task<bool> IsCoachAcceptingNewClientsAsync(Guid coachId, CancellationToken cancellationToken = default);
    Task<bool> IsClientAvailableForNewCoachAsync(Guid clientId, CancellationToken cancellationToken = default);
}