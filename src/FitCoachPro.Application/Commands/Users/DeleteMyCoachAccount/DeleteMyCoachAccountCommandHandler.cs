using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Commands.Users.DeleteMyCoachAccount;

public class DeleteMyCoachAccountCommandHandler(
    IUserContextService userContext,
    IUserRepository userRepository,
    IAccountManager accountManager,
    IUnitOfWork unitOfWork,
    ILogger<DeleteMyCoachAccountCommandHandler> logger
    ) : ICommandHandler<DeleteMyCoachAccountCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IAccountManager _accountManager = accountManager;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<DeleteMyCoachAccountCommandHandler> _logger = logger;

    public async Task<Result> ExecuteAsync(DeleteMyCoachAccountCommand command, CancellationToken cancellationToken)
    {

        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Coach)
        {
            _logger.LogWarning(
                "DeleteMyCoachAccount forbidden: User is not a Coach. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        _logger.LogInformation(
            "DeleteMyCoachAccount attempt started. CoachId: {CoachId}",
            currentUser.UserId);

        var coach = await _userRepository.GetCoachByIdAsync(currentUser.UserId, cancellationToken, true);
        if (coach is null)
        {
            _logger.LogWarning(
                "DeleteMyCoachAccount failed: Coach profile not found. CoachId: {CoachId}",
                currentUser.UserId);
            return Result.Fail(DomainErrors.NotFound(nameof(UserProfile)), StatusCodes.Status404NotFound);
        }

        if (coach.Clients.Count > 0)
        {
            _logger.LogWarning(
                "DeleteMyCoachAccount failed: Coach has active clients. CoachId: {CoachId}, ClientsCount: {Count}",
                currentUser.UserId, coach.Clients.Count);
            return Result.Fail(UserErrors.CoachHasActiveClientsDeleteAccount, StatusCodes.Status400BadRequest);
        }

        var deleteAccountResponse = await _accountManager.DeleteAccountAsync(coach, cancellationToken);
        if (!deleteAccountResponse.IsSuccess)
        {
            _logger.LogWarning(
                "DeleteMyCoachAccount failed: Account deletion failed. CoachId: {CoachId}, Errors: {@Errors}",
                currentUser.UserId, deleteAccountResponse.Errors);
            return Result.Fail(deleteAccountResponse.Errors!, deleteAccountResponse.StatusCode);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "DeleteMyCoachAccount succeeded. CoachId: {CoachId}",
            currentUser.UserId);

        return Result.Success();
    }
}
