using FitCoachPro.Application.Common.Models.Auth;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;

namespace FitCoachPro.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<UserProfile?> GetByAppUserIdAndRoleAsync(Guid userId, UserRole role, CancellationToken cancellationToken = default);
    Task<Admin?> GetAdminById(Guid id, CancellationToken cancellationToken = default, bool track = false);
    Task<Coach?> GetCoachById(Guid id, CancellationToken cancellationToken = default, bool track = false);
    Task<Client?> GetClientById(Guid id, CancellationToken cancellationToken = default, bool track = false);
    IQueryable<UserProfile> GetAllAsQuery(UserRole role);

    Task<Guid> CreateAsync(CreateUserModel model, CancellationToken cancellationToken = default);

    Task<bool> CanCoachAccessClientAsync(Guid coachId, Guid clientId, CancellationToken cancellationToken = default);
}