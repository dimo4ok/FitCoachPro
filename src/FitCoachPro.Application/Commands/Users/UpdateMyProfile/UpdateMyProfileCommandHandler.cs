using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.AspNetCore.Http;

namespace FitCoachPro.Application.Commands.Users.UpdateMyProfile;

public class UpdateMyProfileCommandHandler(
    IUserContextService userContext,
    IUsersAccessService accessService,
    IAccountManager accountManager,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<UpdateMyProfileCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUsersAccessService _accessService = accessService;
    private readonly IAccountManager _accountManager = accountManager;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> ExecuteAsync(UpdateMyProfileCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (!_accessService.HasCurrentUserAccessToUpdateProfile(currentUser.Role))
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var user = await _userRepository.GetUserByIdAndRoleAsync(currentUser.UserId, currentUser.Role, cancellationToken, true);
        if (user is null)
            return Result.Fail(DomainErrors.NotFound(nameof(UserProfile)), StatusCodes.Status404NotFound);

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var emailUpdateResult = await _accountManager.UpdateEmailAsync(user.User, command.Model.Email);
            if (!emailUpdateResult.IsSuccess)
                return Result.Fail(emailUpdateResult.Errors!, emailUpdateResult.StatusCode);

            var phoneUpdateResult = await _accountManager.UpdatePhoneNumberAsync(user.User, command.Model.PhoneNumber);
            if (!phoneUpdateResult.IsSuccess)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Fail(phoneUpdateResult.Errors!, phoneUpdateResult.StatusCode);
            }

            user.FirstName = command.Model.FirstName;
            user.LastName = command.Model.LastName;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Fail(SystemErrors.TransactionFailed, StatusCodes.Status500InternalServerError);
        }
    }
}