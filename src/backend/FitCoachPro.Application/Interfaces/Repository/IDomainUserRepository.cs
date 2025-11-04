using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;

namespace FitCoachPro.Application.Interfaces.Repository;

public interface IDomainUserRepository
{
    Task<UserProfile?> GetByAppUserIdAndRoleAsync(Guid appUserId, UserRole role, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(CreateDomainUserModel model, CancellationToken cancellationToken = default);
}