using FitCoachPro.Application.Common.Models.Auth;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;

namespace FitCoachPro.Application.Interfaces.Repository;

public interface IUserRepository
{
    Task<UserProfile?> GetByAppUserIdAndRoleAsync(Guid userId, UserRole role, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(CreateUserModel model, CancellationToken cancellationToken = default);

    Task<bool> CoachOwnsClientAsync(Guid coachId, Guid clientId, CancellationToken cancellationToken = default);
}