using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities;
using FitCoachPro.Domain.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Commands.ClientCoachRequests.CancelClientCoachRequest;

public class CancelClientCoachRequestCommandHandler(
    IUserContextService userContext,
    IUnitOfWork unitOfWork,
    IClientCoachRequestRepository requestRepository,
    IClientCoachRequestAccessService accessService,
    ILogger<CancelClientCoachRequestCommandHandler> logger
    ) : ICommandHandler<CancelClientCoachRequestCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IClientCoachRequestRepository _requestRepository = requestRepository;
    private readonly IClientCoachRequestAccessService _accessService = accessService;
    private readonly ILogger<CancelClientCoachRequestCommandHandler> _logger = logger;

    public async Task<Result> ExecuteAsync(CancelClientCoachRequestCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        _logger.LogInformation("Attempting to cancel CoachRequest: {RequestId} by User: {UserId}", 
            command.RequestId, currentUser.UserId);

        if (currentUser.Role != UserRole.Client)
        {
            _logger.LogWarning("CancelRequest forbidden: User {UserId} is not a Client", currentUser.UserId);

            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        var request = await _requestRepository.GetByIdAsync(command.RequestId, cancellationToken, track: true);
        if (request == null)
        {
            _logger.LogWarning("CancelRequest failed: Request {RequestId} not found", command.RequestId);

            return Result.Fail(DomainErrors.NotFound(nameof(ClientCoachRequest)));
        }

        if (!_accessService.HasAccesToRequest(request, currentUser))
        {
            _logger.LogWarning("Security Violation: User {UserId} attempted to cancel request {RequestId} they do not own",
                currentUser.UserId, command.RequestId);

            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        if (request.Status != CoachRequestStatus.Pending)
        {
            _logger.LogWarning("CancelRequest failed: Request {RequestId} is already {Status}",
                command.RequestId, request.Status);

            return Result.Fail(ClientCoachRequestErrors.CannotUpdateFinalizedRequest, StatusCodes.Status409Conflict);
        }

        _requestRepository.Delete(request);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully cancelled CoachRequest: {RequestId} for Client: {UserId}",
            command.RequestId, currentUser.UserId);

        return Result.Success(StatusCodes.Status204NoContent);
    }
}
