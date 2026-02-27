using FitCoachPro.Application.Common.Models.Auth;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Identity;
using FitCoachPro.Domain.Entities.Users;
using FitCoachPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Infrastructure.Repositories;

public class UserRepository(
    AppDbContext dbContext, 
    ILogger<UserRepository> logger
    ) : IUserRepository
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly ILogger<UserRepository> _logger = logger;

    public async Task<Guid?> GetIdByAppUserIdAndRoleAsync(Guid userId, UserRole role, CancellationToken cancellationToken = default) =>
        role switch
        {
            UserRole.Admin => await _dbContext.Admins
                .Where(x => x.UserId == userId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken),
            UserRole.Coach => await _dbContext.Coaches
                .Where(x => x.UserId == userId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken),
            UserRole.Client => await _dbContext.Clients
                .Where(x => x.UserId == userId)
                .Select(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken),
            _ => null
        };

    public IQueryable<UserProfile> GetAllUsersByRoleAsQuery(UserRole role)
    {
        IQueryable<UserProfile> query = role switch
        {
            UserRole.Admin => _dbContext.Admins,
            UserRole.Coach => _dbContext.Coaches,
            UserRole.Client => _dbContext.Clients,
            _ => Enumerable.Empty<UserProfile>().AsQueryable()
        };

        return query
            .AsNoTracking()
            .Include(x => x.User)
            .OrderBy(x => x.CreatedAt);
    }

    public async Task<UserProfile?> GetUserByIdAndRoleAsync(Guid id, UserRole role, CancellationToken cancellationToken = default, bool track = false)
    {
        IQueryable<UserProfile> query = role switch
        {
            UserRole.Admin => _dbContext.Admins,
            UserRole.Coach => _dbContext.Coaches,
            UserRole.Client => _dbContext.Clients,
            _ => Enumerable.Empty<UserProfile>().AsQueryable()
        };

        query = track
            ? query
            : query.AsNoTracking();

        return await query
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public IQueryable<Client> GetAllCoachClientsAsQuery(Guid id) =>
        _dbContext.Clients
            .AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.CoachId == id);

    public async Task<Admin?> GetAdminByIdAsync(Guid id, CancellationToken cancellationToken = default, bool track = false)
    {
        var query = track
            ? _dbContext.Admins
            : _dbContext.Admins.AsNoTracking();

        return await query
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Coach?> GetCoachByIdAsync(Guid id, CancellationToken cancellationToken = default, bool track = false)
    {
        var query = track
            ? _dbContext.Coaches
            : _dbContext.Coaches.AsNoTracking();

        return await query
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Client?> GetClientByIdAsync(Guid id, CancellationToken cancellationToken = default, bool track = false)
    {
        var query = track
            ? _dbContext.Clients
            : _dbContext.Clients.AsNoTracking();

        return await query
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Client?> GetClientByIdWithCoachAsync(Guid id, CancellationToken cancellationToken = default, bool track = false)
    {
        var query = track
            ? _dbContext.Clients
            : _dbContext.Clients.AsNoTracking();

        return await query
            .Include(x => x.Coach)
                .ThenInclude(c => c!.Clients)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Guid> CreateAsync(CreateUserModel model, CancellationToken cancellationToken = default)
    {
        Guid domainUserId;

        switch (model.Role)
        {
            case (UserRole.Admin):
                var admin = new Admin { UserId = model.UserId, FirstName = model.FirstName, LastName = model.LastName };
                await _dbContext.Admins.AddAsync(admin, cancellationToken);
                domainUserId = admin.Id;
                break;
            case (UserRole.Coach):
                var coach = new Coach { UserId = model.UserId, FirstName = model.FirstName, LastName = model.LastName };
                await _dbContext.Coaches.AddAsync(coach, cancellationToken);
                domainUserId = coach.Id;
                break;
            case (UserRole.Client):
                var client = new Client { UserId = model.UserId, FirstName = model.FirstName, LastName = model.LastName };
                await _dbContext.Clients.AddAsync(client, cancellationToken);
                domainUserId = client.Id;
                break;
            default:
                _logger.LogError("CreateAsync failed: Unknown UserRole '{Role}' for UserId {UserId}",
                    model.Role, model.UserId);
                throw new ArgumentOutOfRangeException(nameof(model), "Unknown user role");
        }

        return domainUserId;
    }

    public void Delete(User user) =>
        _dbContext.Users.Remove(user);

    public async Task<bool> CanCoachAccessClientAsync(Guid coachId, Guid clientId, CancellationToken cancellationToken = default) =>
        await _dbContext.Clients.AnyAsync(
            x => x.Id == clientId &&
            x.CoachId == coachId,
            cancellationToken);
}