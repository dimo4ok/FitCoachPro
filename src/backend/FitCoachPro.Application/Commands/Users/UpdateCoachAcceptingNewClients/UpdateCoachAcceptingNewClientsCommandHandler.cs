using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.AspNetCore.Http;

namespace FitCoachPro.Application.Commands.Users.UpdateCoachAcceptingNewClients;

public class UpdateCoachAcceptingNewClientsCommandHandler(
    IUserContextService userContext,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<UpdateCoachAcceptingNewClientsCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> ExecuteAsync(UpdateCoachAcceptingNewClientsCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Coach)
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var coach = await _userRepository.GetCoachByIdAsync(currentUser.UserId, cancellationToken, true);
        if (coach is null)
            return Result.Fail(DomainErrors.NotFound(nameof(Coach)));

        coach.AcceptanceStatus = command.AcceptanceStatus;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
