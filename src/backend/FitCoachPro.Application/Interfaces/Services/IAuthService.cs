using FitCoachPro.Application.Common.Models.Response;
using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Infrastructure.Services
{
    public interface IAuthService
    {
        Task<Result<AuthModel>> RegisterUserAsync(CreateUserModel model, CancellationToken cancellationToken = default);
        Task<Result<AuthModel>> LoginUserAsync(LoginUserModel model, CancellationToken cancellationToken = default);
    }
}