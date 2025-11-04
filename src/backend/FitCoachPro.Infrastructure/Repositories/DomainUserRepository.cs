using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Application.Interfaces.Repository;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;
using FitCoachPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Infrastructure.Repositories;

public class DomainUserRepository : IDomainUserRepository
{
    private readonly AppDbContext _dbContext;

    public DomainUserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserProfile?> GetByAppUserIdAndRoleAsync(Guid appUserId, UserRole role, CancellationToken cancellationToken = default)
    {
        return role switch
        {
            UserRole.Admin => await _dbContext.Admins.FirstOrDefaultAsync(x => x.UserId == appUserId, cancellationToken),
            UserRole.Coach => await _dbContext.Coaches.FirstOrDefaultAsync(x => x.UserId == appUserId, cancellationToken),
            UserRole.Client => await _dbContext.Clients.FirstOrDefaultAsync(x => x.UserId == appUserId, cancellationToken),
            _ => null
        };
    }

    public async Task<Guid> CreateAsync(CreateDomainUserModel model, CancellationToken cancellationToken = default)
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
                throw new ArgumentOutOfRangeException(nameof(model.Role), "Unknown user role");
        }

        return domainUserId;
    }
}
