using FitCoachPro.Domain.Entities.Users;

namespace FitCoachPro.Application.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task<User> GetUserById(Guid id, CancellationToken cancellationToken = default);
    }
}