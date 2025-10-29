using FitCoachPro.Application.Common.Models.Response;
using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Domain.Entities.Users;

namespace FitCoachPro.Application.Interfaces.Services
{
    public interface IAppUserService
    {
        Task<Result<bool>> CreateUserAsync(User domainUser, CreateUserModel model);
        Task<Result<AppUserModel>> AuthenticateUserAsync(LoginUserModel model, CancellationToken cancellationToken = default);
    }
}