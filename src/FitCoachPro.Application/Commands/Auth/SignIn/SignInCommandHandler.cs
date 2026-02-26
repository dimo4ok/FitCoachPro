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
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Commands.Auth.SignIn;

public class SignInCommandHandler(
    UserManager<User> userManager,
    IUserRepository domainUserRepository,
    IAuthHelper authHelper,
    ILogger<SignInCommandHandler> logger
    ) : ICommandHandler<SignInCommand, Result<AuthModel>>
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly IUserRepository _userRepository = domainUserRepository;
    private readonly IAuthHelper _authHelper = authHelper;
    private readonly ILogger<SignInCommandHandler> _logger = logger;

    public async Task<Result<AuthModel>> ExecuteAsync(SignInCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("SignIn attempt started for user: {UserName}", command.Model.UserName);

        var user = await _userManager.FindByNameAsync(command.Model.UserName);
        if (user == null)
        {
            _logger.LogWarning("SignIn failed: User {UserName} not found. Error: {@Error}",
                command.Model.UserName, UserErrors.NotFound);
            return Result<AuthModel>.Fail(UserErrors.NotFound);
        }

        var validPassword = await _userManager.CheckPasswordAsync(user, command.Model.Password);
        if (!validPassword)
        {
            _logger.LogWarning("SignIn failed: Invalid password for user {UserName}",
                command.Model.UserName);
            return Result<AuthModel>.Fail(UserErrors.InvalidCredentials, StatusCodes.Status401Unauthorized);
        }

        var roleString = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
        if (string.IsNullOrEmpty(roleString))
        {
            _logger.LogError("Critical Error: User {UserId} has no assigned identity roles",
                user.Id);
            return Result<AuthModel>.Fail(UserErrors.RoleNotFound, StatusCodes.Status500InternalServerError);
        }

        if (!Enum.TryParse<UserRole>(roleString, true, out var userRole))
        {
            _logger.LogError("Critical Error: Role {Role} for User {UserId} is not a valid UserRole enum",
                roleString, user.Id);
            return Result<AuthModel>.Fail(UserErrors.InvalidRole, StatusCodes.Status500InternalServerError);
        }

        var domainUserId = await _userRepository.GetIdByAppUserIdAndRoleAsync(user.Id, userRole, cancellationToken);
        if (domainUserId == null)
        {
            _logger.LogWarning("SignIn failed: Domain profile missing for Identity User {UserId} with role {Role}",
                user.Id, userRole);
            return Result<AuthModel>.Fail(UserErrors.NotFound);
        }

        _logger.LogInformation("SignIn successful for User: {UserName} (ID: {DomainId})",
            user.UserName, domainUserId);

        var jwtPayloadModel = new JwtPayloadModel
        {
            Id = domainUserId.Value,
            UserName = user.UserName!,
            Role = userRole,
        };

        var authModel = _authHelper.GenerateTokenByData(jwtPayloadModel);

        _logger.LogInformation("User {UserName} successfully authenticated. DomainId: {DomainId}, Role: {UserRole}, Expires: {Expiration}",
            user.UserName ,domainUserId, userRole, authModel.Expires);

        return Result<AuthModel>.Success(authModel);
    }
}
