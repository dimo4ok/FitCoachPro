using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Commands.Users.UnassignClient;

public class UnassignClientCommandHandler(
    IUserContextService userContext,
    IUserRepository userRepository,
    ICoachAssignmentService coachAssignmentService,
    IUnitOfWork unitOfWork,
    ILogger<UnassignClientCommandHandler> logger
    ) : ICommandHandler<UnassignClientCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ICoachAssignmentService _coachAssignmentService = coachAssignmentService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<UnassignClientCommandHandler> _logger = logger;

    public async Task<Result> ExecuteAsync(UnassignClientCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Coach)
        {
            _logger.LogWarning(
                "UnassignClient forbidden: User is not a Coach. CoachId: {CoachId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        _logger.LogInformation(
            "UnassignClient attempt started. CoachId: {CoachId}, ClientId: {ClientId}",
            currentUser.UserId, command.ClientId);

        if(!await _userRepository.CanCoachAccessClientAsync(currentUser.UserId, command.ClientId, cancellationToken))
        {
            _logger.LogWarning(
                "UnassignClient forbidden: Coach has no access to client. CoachId: {CoachId}, ClientId: {ClientId}",
                currentUser.UserId, command.ClientId);
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        var unassignResult = await _coachAssignmentService.UnassignCoachAsync(command.ClientId, cancellationToken);
        if(!unassignResult.IsSuccess)
        {
            _logger.LogWarning(
                "UnassignClient failed: Unassign operation failed. CoachId: {CoachId}, ClientId: {ClientId}, Errors: {@Errors}",
                currentUser.UserId, command.ClientId, unassignResult.Errors);
            return Result.Fail(unassignResult.Errors!, unassignResult.StatusCode);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "UnassignClient succeeded. CoachId: {CoachId}, ClientId: {ClientId}",
            currentUser.UserId, command.ClientId);

        return Result.Success();
    }
}
