using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.AspNetCore.Http;

namespace FitCoachPro.Application.Commands.Users.UpdateMyProfilePassword;

public class UpdateMyProfilePasswordCommandHandler(
    IUserContextService contextService,
    IUserRepository userRepository,
    IAccountManager accountManager,
    IUsersAccessService usersAccess
    ) : ICommandHandler<UpdateMyProfilePasswordCommand, Result>
{
    private readonly IUserContextService _contextService = contextService;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IAccountManager _accountManager = accountManager;
    private readonly IUsersAccessService _usersAccess = usersAccess;

    public async Task<Result> ExecuteAsync(UpdateMyProfilePasswordCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _contextService.Current;
        if (!_usersAccess.HasCurrentUserAccessToUpdateProfile(currentUser.Role))
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var user = await _userRepository.GetUserByIdAndRoleAsync(currentUser.UserId, currentUser.Role, cancellationToken, true);
        if (user is null)
            return Result.Fail(DomainErrors.NotFound(nameof(UserProfile)), StatusCodes.Status404NotFound);

        var updatePasswordResult = await _accountManager.UpdatePasswordAsync(user.User, command.Model);
        if (!updatePasswordResult.IsSuccess)
            return Result.Fail(updatePasswordResult.Errors!, updatePasswordResult.StatusCode);

        return Result.Success();
    }
}