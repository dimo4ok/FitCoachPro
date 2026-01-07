using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.AspNetCore.Http;

namespace FitCoachPro.Application.Commands.Users.DeleteMyCoachAccount;

public class DeleteMyCoachAccountCommandHandler(
    IUserContextService userContext,
    IUserRepository userRepository,
    IAccountManager accountManager,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<DeleteMyCoachAccountCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IAccountManager _accountManager = accountManager;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> ExecuteAsync(DeleteMyCoachAccountCommand command, CancellationToken cancellationToken)
    {

        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Coach)
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var coach = await _userRepository.GetCoachByIdAsync(currentUser.UserId, cancellationToken, true);
        if (coach is null)
            return Result.Fail(DomainErrors.NotFound(nameof(UserProfile)), StatusCodes.Status404NotFound);

        if (coach.Clients.Count > 0)
            return Result.Fail(UserErrors.CoachHasActiveClientsDeleteAccount, StatusCodes.Status400BadRequest);

        var deleteAccountResponse = await _accountManager.DeleteAccountAsync(coach, cancellationToken);
        if (!deleteAccountResponse.IsSuccess)
            return Result.Fail(deleteAccountResponse.Errors!, deleteAccountResponse.StatusCode);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
