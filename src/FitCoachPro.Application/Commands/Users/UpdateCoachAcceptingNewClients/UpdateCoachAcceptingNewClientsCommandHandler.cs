using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Commands.Users.UpdateCoachAcceptingNewClients;

public class UpdateCoachAcceptingNewClientsCommandHandler(
    IUserContextService userContext,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ILogger<UpdateCoachAcceptingNewClientsCommandHandler> logger
    ) : ICommandHandler<UpdateCoachAcceptingNewClientsCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<UpdateCoachAcceptingNewClientsCommandHandler> _logger = logger;

    public async Task<Result> ExecuteAsync(UpdateCoachAcceptingNewClientsCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Coach)
        {
            _logger.LogWarning(
                "UpdateCoachAcceptingNewClients forbidden: User is not a Coach. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        _logger.LogInformation(
            "UpdateCoachAcceptingNewClients attempt started. CoachId: {CoachId}, NewAcceptanceStatus: {AcceptanceStatus}",
            currentUser.UserId, command.AcceptanceStatus);

        var coach = await _userRepository.GetCoachByIdAsync(currentUser.UserId, cancellationToken, true);
        if (coach is null)
        {
            _logger.LogWarning(
                "UpdateCoachAcceptingNewClients failed: Coach profile not found. CoachId: {CoachId}",
                currentUser.UserId);
            return Result.Fail(DomainErrors.NotFound(nameof(Coach)));
        }

        coach.AcceptanceStatus = command.AcceptanceStatus;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "UpdateCoachAcceptingNewClients succeeded. CoachId: {CoachId}, AcceptanceStatus: {AcceptanceStatus}",
            currentUser.UserId, coach.AcceptanceStatus);

        return Result.Success();
    }
}
