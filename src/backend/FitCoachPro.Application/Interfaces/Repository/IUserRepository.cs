using FitCoachPro.Application.Common.Models;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;

namespace FitCoachPro.Application.Interfaces.Repository;

public interface IUserRepository
{
    Task<UserProfile?> GetByAppUserIdAndRoleAsync(Guid userId, UserRole role, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(CreateUserModel model, CancellationToken cancellationToken = default);
}