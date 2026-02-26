using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Commands.Users.UpdateMyProfilePassword;

public class UpdateMyProfilePasswordCommandHandler(
    IUserContextService contextService,
    IUserRepository userRepository,
    IAccountManager accountManager,
    IUsersAccessService usersAccess,
    ILogger<UpdateMyProfilePasswordCommandHandler> logger
    ) : ICommandHandler<UpdateMyProfilePasswordCommand, Result>
{
    private readonly IUserContextService _contextService = contextService;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IAccountManager _accountManager = accountManager;
    private readonly IUsersAccessService _usersAccess = usersAccess;
    private readonly ILogger<UpdateMyProfilePasswordCommandHandler> _logger = logger;

    public async Task<Result> ExecuteAsync(UpdateMyProfilePasswordCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _contextService.Current;
        if (!_usersAccess.HasCurrentUserAccessToUpdateProfile(currentUser.Role))
        {
            _logger.LogWarning(
                "UpdateMyProfilePassword forbidden. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        _logger.LogInformation(
            "UpdateMyProfilePassword attempt started. UserId: {UserId}, Role: {Role}",
            currentUser.UserId, currentUser.Role);

        var user = await _userRepository.GetUserByIdAndRoleAsync(currentUser.UserId, currentUser.Role, cancellationToken, true);
        if (user is null)
        {
            _logger.LogWarning(
                "UpdateMyProfilePassword failed: Profile not found. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result.Fail(DomainErrors.NotFound(nameof(UserProfile)), StatusCodes.Status404NotFound);
        }

        var updatePasswordResult = await _accountManager.UpdatePasswordAsync(user.User, command.Model);
        if (!updatePasswordResult.IsSuccess)
        {
            _logger.LogWarning(
                "UpdateMyProfilePassword failed: Password update failed. UserId: {UserId}, Errors: {@Errors}",
                currentUser.UserId, updatePasswordResult.Errors);
            return Result.Fail(updatePasswordResult.Errors!, updatePasswordResult.StatusCode);
        }

        _logger.LogInformation(
            "UpdateMyProfilePassword succeeded. UserId: {UserId}, Role: {Role}",
            currentUser.UserId, currentUser.Role);

        return Result.Success();
    }
}