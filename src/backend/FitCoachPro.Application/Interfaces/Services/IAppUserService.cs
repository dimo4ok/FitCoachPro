using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Interfaces.Services;

public interface IAppUserService
{
    Task<Result<Guid>> CreateAsync(SignUpModel model);
    Task<Result<AuthUserModel>> AuthenticateAsync(SignInModel model);
}