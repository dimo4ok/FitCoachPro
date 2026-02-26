using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Commands.Users.DeleteMyClientAccount;

public class DeleteMyClientAccountCommandHandler(
    IUserContextService userContext,
    IUserRepository userRepository,
    IAccountManager accountManager,
    IUnitOfWork unitOfWork,
    ILogger<DeleteMyClientAccountCommandHandler> logger
    ) : ICommandHandler<DeleteMyClientAccountCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IAccountManager _accountManager = accountManager;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<DeleteMyClientAccountCommandHandler> _logger = logger;

    public async Task<Result> ExecuteAsync(DeleteMyClientAccountCommand command, CancellationToken cancellationToken)
    {

        var currentUser = _userContext.Current;
        if(currentUser.Role != UserRole.Client)
        {
            _logger.LogWarning(
                "DeleteMyClientAccount forbidden: User is not a Client. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        _logger.LogInformation(
            "DeleteMyClientAccount attempt started. ClientId: {ClientId}",
            currentUser.UserId);

        var client = await _userRepository.GetClientByIdAsync(currentUser.UserId, cancellationToken, true);
        if (client is null)
        {
            _logger.LogWarning(
                "DeleteMyClientAccount failed: Client profile not found. ClientId: {ClientId}",
                currentUser.UserId);
            return Result.Fail(DomainErrors.NotFound(nameof(UserProfile)), StatusCodes.Status404NotFound);
        }

        if (client.CoachId != null)
        {
            _logger.LogWarning(
                "DeleteMyClientAccount failed: Client has assigned coach. ClientId: {ClientId}, CoachId: {CoachId}",
                currentUser.UserId, client.CoachId);
            return Result.Fail(UserErrors.ClientHasAssignedCoachDeleteAccount, StatusCodes.Status400BadRequest);
        }

        var deleteAccountResponse = await _accountManager.DeleteAccountAsync(client, cancellationToken);
        if (!deleteAccountResponse.IsSuccess)
        {
            _logger.LogWarning(
                "DeleteMyClientAccount failed: Account deletion failed. ClientId: {ClientId}, Errors: {@Errors}",
                currentUser.UserId, deleteAccountResponse.Errors);
            return Result.Fail(deleteAccountResponse.Errors!, deleteAccountResponse.StatusCode);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "DeleteMyClientAccount succeeded. ClientId: {ClientId}",
            currentUser.UserId);

        return Result.Success();
    }
}
