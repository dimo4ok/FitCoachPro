using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Commands.Users.UnassignCoach;

public class UnassignCoachCommandHandler(
    IUserContextService userContext,
    ICoachAssignmentService coachAssignmentService,
    IUnitOfWork unitOfWork,
    ILogger<UnassignCoachCommandHandler> logger
    ) : ICommandHandler<UnassignCoachCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly ICoachAssignmentService _coachAssignmentService = coachAssignmentService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<UnassignCoachCommandHandler> _logger = logger;

    public async Task<Result> ExecuteAsync(UnassignCoachCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Client)
        {
            _logger.LogWarning(
                "UnassignCoach forbidden: User is not a Client. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        _logger.LogInformation(
            "UnassignCoach attempt started. ClientId: {ClientId}",
            currentUser.UserId);

        var unassignResult = await _coachAssignmentService.UnassignCoachAsync(currentUser.UserId, cancellationToken);
        if(!unassignResult.IsSuccess)
        {
            _logger.LogWarning(
                "UnassignCoach failed: Unassign operation failed. ClientId: {ClientId}, Errors: {@Errors}",
                currentUser.UserId, unassignResult.Errors);
            return Result.Fail(unassignResult.Errors!, unassignResult.StatusCode);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "UnassignCoach succeeded. ClientId: {ClientId}",
            currentUser.UserId);

        return Result.Success();
    }
}
