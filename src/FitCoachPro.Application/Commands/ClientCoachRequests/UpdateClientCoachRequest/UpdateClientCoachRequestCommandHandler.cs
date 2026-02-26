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

namespace FitCoachPro.Application.Commands.ClientCoachRequests.UpdateClientCoachRequest;

public class UpdateClientCoachRequestCommandHandler(
    IUserContextService userContext,
    IUnitOfWork unitOfWork,
    IClientCoachRequestRepository requestRepository,
    ICoachAssignmentService coachAssignmentService,
    IClientCoachRequestAccessService accessService,
    ILogger<UpdateClientCoachRequestCommandHandler> logger
    ) : ICommandHandler<UpdateClientCoachRequestCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IClientCoachRequestRepository _requestRepository = requestRepository;
    private readonly ICoachAssignmentService _coachAssignmentService = coachAssignmentService;
    private readonly IClientCoachRequestAccessService _accessService = accessService;
    private readonly ILogger<UpdateClientCoachRequestCommandHandler> _logger = logger;

    public async Task<Result> ExecuteAsync(UpdateClientCoachRequestCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Coach)
        {
            _logger.LogWarning(
                "UpdateClientCoachRequest forbidden: User is not a Coach. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        _logger.LogInformation(
            "UpdateClientCoachRequest attempt started. RequestId: {RequestId}, CoachId: {CoachId}, NewStatus: {Status}",
            command.RequstId, currentUser.UserId, command.Status);

        var request = await _requestRepository.GetByIdAsync(command.RequstId, cancellationToken, track: true);
        if (request == null)
        {
            _logger.LogWarning(
                "UpdateClientCoachRequest failed: Request not found. RequestId: {RequestId}",
                command.RequstId);
            return Result.Fail(DomainErrors.NotFound(nameof(ClientCoachRequest)));
        }

        if (!_accessService.HasAccesToRequest(request, currentUser))
        {
            _logger.LogWarning(
                "UpdateClientCoachRequest forbidden: Coach has no access to request. RequestId: {RequestId}, CoachId: {CoachId}, OwnerCoachId: {OwnerCoachId}",
                command.RequstId, currentUser.UserId, request.CoachId);
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        if (request.Status != CoachRequestStatus.Pending)
        {
            _logger.LogWarning(
                "UpdateClientCoachRequest failed: Request already finalized. RequestId: {RequestId}, CurrentStatus: {CurrentStatus}",
                command.RequstId, request.Status);
            return Result.Fail(ClientCoachRequestErrors.CannotUpdateFinalizedRequest, StatusCodes.Status409Conflict);
        }

        request.Status = command.Status;
        request.ReviewedAt = DateTime.UtcNow;

        if (command.Status != CoachRequestStatus.Accepted)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "UpdateClientCoachRequest succeeded (non-accept). RequestId: {RequestId}, CoachId: {CoachId}, NewStatus: {Status}",
                request.Id, currentUser.UserId, request.Status);

            return Result.Success();
        }

        var clientUpdateResult = await _coachAssignmentService.AssignCoachToClientAsync(request.ClientId, request.CoachId, cancellationToken);
        if (!clientUpdateResult.IsSuccess)
        {
            _logger.LogWarning(
                "UpdateClientCoachRequest failed: Could not assign coach to client. RequestId: {RequestId}, ClientId: {ClientId}, CoachId: {CoachId}, Errors: {@Errors}",
                request.Id, request.ClientId, request.CoachId, clientUpdateResult.Errors);
            return Result.Fail(clientUpdateResult.Errors!, clientUpdateResult.StatusCode);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "UpdateClientCoachRequest succeeded (accepted). RequestId: {RequestId}, ClientId: {ClientId}, CoachId: {CoachId}",
            request.Id, request.ClientId, request.CoachId);

        return Result.Success();
    }
}