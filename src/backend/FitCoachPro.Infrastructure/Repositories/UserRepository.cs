using FitCoachPro.Application.Common.Models.Auth;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;
using FitCoachPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FitCoachPro.Infrastructure.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<UserProfile?> GetByAppUserIdAndRoleAsync(Guid userId, UserRole role, CancellationToken cancellationToken = default)
        => role switch
        {
            UserRole.Admin => await _dbContext.Admins.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken),
            UserRole.Coach => await _dbContext.Coaches.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken),
            UserRole.Client => await _dbContext.Clients.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken),
            _ => null
        };

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
                throw new ArgumentOutOfRangeException(nameof(model), "Unknown user role");
        }

        return domainUserId;
    }

    public async Task<bool> CanCoachAccessClientAsync(Guid coachId, Guid clientId, CancellationToken cancellationToken = default)
        => await _dbContext.Clients.AnyAsync(
            x => x.Id == clientId
            && x.CoachId == coachId,
            cancellationToken);
}
