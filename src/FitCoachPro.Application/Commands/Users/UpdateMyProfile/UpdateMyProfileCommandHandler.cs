using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Commands.Users.UpdateMyProfile;

public class UpdateMyProfileCommandHandler(
    IUserContextService userContext,
    IUsersAccessService accessService,
    IAccountManager accountManager,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ILogger<UpdateMyProfileCommandHandler> logger
    ) : ICommandHandler<UpdateMyProfileCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUsersAccessService _accessService = accessService;
    private readonly IAccountManager _accountManager = accountManager;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<UpdateMyProfileCommandHandler> _logger = logger;

    public async Task<Result> ExecuteAsync(UpdateMyProfileCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (!_accessService.HasCurrentUserAccessToUpdateProfile(currentUser.Role))
        {
            _logger.LogWarning(
                "UpdateMyProfile forbidden. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        _logger.LogInformation(
            "UpdateMyProfile attempt started. UserId: {UserId}, Role: {Role}, NewEmail: {Email}",
            currentUser.UserId, currentUser.Role, command.Model.Email);

        var user = await _userRepository.GetUserByIdAndRoleAsync(currentUser.UserId, currentUser.Role, cancellationToken, true);
        if (user is null)
        {
            _logger.LogWarning(
                "UpdateMyProfile failed: Profile not found. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result.Fail(DomainErrors.NotFound(nameof(UserProfile)), StatusCodes.Status404NotFound);
        }

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var emailUpdateResult = await _accountManager.UpdateEmailAsync(user.User, command.Model.Email);
            if (!emailUpdateResult.IsSuccess)
            {
                _logger.LogWarning(
                    "UpdateMyProfile failed: Email update failed. UserId: {UserId}, Errors: {@Errors}",
                    currentUser.UserId, emailUpdateResult.Errors);
                return Result.Fail(emailUpdateResult.Errors!, emailUpdateResult.StatusCode);
            }

            var phoneUpdateResult = await _accountManager.UpdatePhoneNumberAsync(user.User, command.Model.PhoneNumber);
            if (!phoneUpdateResult.IsSuccess)
            {
                _logger.LogWarning(
                    "UpdateMyProfile failed: Phone update failed. UserId: {UserId}, Errors: {@Errors}",
                    currentUser.UserId, phoneUpdateResult.Errors);
                await transaction.RollbackAsync(cancellationToken);
                return Result.Fail(phoneUpdateResult.Errors!, phoneUpdateResult.StatusCode);
            }

            user.FirstName = command.Model.FirstName;
            user.LastName = command.Model.LastName;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "UpdateMyProfile succeeded. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "UpdateMyProfile transaction failed. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            await transaction.RollbackAsync(cancellationToken);
            return Result.Fail(SystemErrors.TransactionFailed, StatusCodes.Status500InternalServerError);
        }
    }
}