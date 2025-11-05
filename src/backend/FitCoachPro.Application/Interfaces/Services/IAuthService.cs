using FitCoachPro.Application.Common.Models;
using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Infrastructure.Services;

public interface IAuthService
{
    Task<Result<AuthModel>> SignUpAsync(SignUpModel model, CancellationToken cancellationToken = default);
    Task<Result<AuthModel>> SignInAsync(SignInModel model, CancellationToken cancellationToken = default);
}