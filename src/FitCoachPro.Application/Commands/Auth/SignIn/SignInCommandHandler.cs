using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.Auth;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Helpers;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace FitCoachPro.Application.Commands.Auth.SignIn;

public class SignInCommandHandler(
    UserManager<User> userManager,
    IUserRepository domainUserRepository,
    IAuthHelper authHelper
    ) : ICommandHandler<SignInCommand, Result<AuthModel>>
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly IUserRepository _userRepository = domainUserRepository;
    private readonly IAuthHelper _authHelper = authHelper;

    public async Task<Result<AuthModel>> ExecuteAsync(SignInCommand command, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(command.Model.UserName);
        if (user == null)
            return Result<AuthModel>.Fail(UserErrors.NotFound);

        var validPassword = await _userManager.CheckPasswordAsync(user, command.Model.Password);
        if (!validPassword)
            return Result<AuthModel>.Fail(UserErrors.InvalidCredentials, StatusCodes.Status401Unauthorized);

        var roleString = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
        if (string.IsNullOrEmpty(roleString))
            return Result<AuthModel>.Fail(UserErrors.RoleNotFound, StatusCodes.Status500InternalServerError);

        if (!Enum.TryParse<UserRole>(roleString, true, out var userRole))
            return Result<AuthModel>.Fail(UserErrors.InvalidRole, StatusCodes.Status500InternalServerError);

        var domainUserId = await _userRepository.GetIdByAppUserIdAndRoleAsync(user.Id, userRole, cancellationToken);
        if (domainUserId == null)
            return Result<AuthModel>.Fail(UserErrors.NotFound);

        var jwtPayloadModel = new JwtPayloadModel
        {
            Id = domainUserId.Value,
            UserName = user.UserName!,
            Role = userRole,
        };

        var authModel = _authHelper.GenerateTokenByData(jwtPayloadModel);

        return Result<AuthModel>.Success(authModel);
    }
}
