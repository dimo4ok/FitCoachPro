using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Helpers;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.AspNetCore.Http;

namespace FitCoachPro.Application.Commands.Users.DeleteMyClientAccount;

public class DeleteMyClientAccountCommandHandler(
    IUserContextService userContext,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IUserHelper userHelper
    ) : ICommandHandler<DeleteMyClientAccountCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserHelper _userHelper = userHelper;

    public async Task<Result> ExecuteAsync(DeleteMyClientAccountCommand command, CancellationToken cancellationToken)
    {

        var currentUser = _userContext.Current;
        if(currentUser.Role != UserRole.Client)
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var client = await _userRepository.GetClientByIdAsync(currentUser.UserId, cancellationToken, true);
        if (client is null)
            return Result.Fail(DomainErrors.NotFound(nameof(UserProfile)), StatusCodes.Status404NotFound);

        if (client.CoachId != null)
            return Result.Fail(UserErrors.ClientHasAssignedCoachDeleteAccount, StatusCodes.Status400BadRequest);

        var deleteAccountResponse = await _userHelper.DeleteAccountAsync(client, cancellationToken);
        if (!deleteAccountResponse.IsSuccess)
            return Result.Fail(deleteAccountResponse.Errors!, deleteAccountResponse.StatusCode);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
