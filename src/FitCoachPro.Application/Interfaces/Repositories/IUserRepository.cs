using FitCoachPro.Application.Common.Models.Auth;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Identity;
using FitCoachPro.Domain.Entities.Users;

namespace FitCoachPro.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<Guid?> GetIdByAppUserIdAndRoleAsync(Guid userId, UserRole role, CancellationToken cancellationToken = default);
    IQueryable<UserProfile?> GetAllUsersByRoleAsQuery(UserRole role);

    Task<Admin?> GetAdminByIdAsync(Guid id, CancellationToken cancellationToken = default, bool track = false);

    Task<Coach?> GetCoachByIdAsync(Guid id, CancellationToken cancellationToken = default, bool track = false);
    IQueryable<Client> GetAllCoachClientsAsQuery(Guid id);

    Task<Client?> GetClientByIdAsync(Guid id, CancellationToken cancellationToken = default, bool track = false);
    Task<Client?> GetClientByIdWithCoachAsync(Guid id, CancellationToken cancellationToken = default, bool track = false);

    Task<Guid> CreateAsync(CreateUserModel model, CancellationToken cancellationToken = default);
    void Delete(User user);

    Task<bool> CanCoachAccessClientAsync(Guid coachId, Guid clientId, CancellationToken cancellationToken = default);
    Task<UserProfile?> GetUserByIdAndRoleAsync(Guid id, UserRole role, CancellationToken cancellationToken = default, bool track = false);
}