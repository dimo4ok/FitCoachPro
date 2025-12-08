using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Domain.Entities;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Infrastructure.Repositories;

public class ClientCoachRequestRepository(AppDbContext dbContext) : IClientCoachRequestRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<ClientCoachRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default, bool track = false)
    {
        var query = track
            ? _dbContext.ClientCoachRequests
            : _dbContext.ClientCoachRequests.AsNoTracking();

        return await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public IQueryable<ClientCoachRequest> GetAllByUserIdAsQuery(Guid userId) =>
    _dbContext.ClientCoachRequests
        .AsNoTracking()
        .Where(x => x.CoachId == userId || x.ClientId == userId)
        .OrderBy(x => x.CreatedAt);

    public IQueryable<ClientCoachRequest> GetAllByUserIdAndUserRoleAsQuery(Guid userId, UserRole role, bool track = false)
    {
        var query = track
            ? _dbContext.ClientCoachRequests.OrderBy(x => x.CreatedAt)
            : _dbContext.ClientCoachRequests.AsNoTracking().OrderBy(x => x.CreatedAt);

        return role switch
        {
            UserRole.Client => query.Where(x => x.ClientId == userId),
            UserRole.Coach => query.Where(x => x.CoachId == userId),
            _ => Enumerable.Empty<ClientCoachRequest>().AsQueryable(),
        };
    }

    public async Task CreateAsync(ClientCoachRequest request, CancellationToken cancellationToken = default) =>
        await _dbContext.ClientCoachRequests.AddAsync(request, cancellationToken);

    public void Delete(ClientCoachRequest request) =>
        _dbContext.ClientCoachRequests.Remove(request);

    public void DeleteRequests(IEnumerable<ClientCoachRequest> requests) =>
        _dbContext.ClientCoachRequests.RemoveRange(requests);

    public async Task<bool> IsDuplicateRequestAsync(Guid clientId, Guid coachId, CancellationToken cancellationToken = default) =>
        await _dbContext.ClientCoachRequests.AnyAsync(x =>
            x.ClientId == clientId &&
            x.CoachId == coachId &&
            x.Status == CoachRequestStatus.Pending,
            cancellationToken);

    public async Task<bool> IsCoachAcceptingNewClientsAsync(Guid coachId, CancellationToken cancellationToken = default) =>
        await _dbContext.Coaches.AnyAsync(x => x.Id == coachId && x.IsAcceptingNewClients == true, cancellationToken);

    public async Task<bool> IsClientAvailableForNewCoachAsync(Guid clientId, CancellationToken cancellationToken = default) =>
        await _dbContext.Clients.AnyAsync(x => x.Id == clientId && x.CoachId == null, cancellationToken);
}
