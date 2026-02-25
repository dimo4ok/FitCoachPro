using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities;
using FitCoachPro.Domain.Entities.Enums;
using Microsoft.AspNetCore.Http;

namespace FitCoachPro.Application.Commands.ClientCoachRequests.UpdateClientCoachRequest;

public class UpdateClientCoachRequestCommandHandler(
    IUserContextService userContext,
    IUnitOfWork unitOfWork,
    IClientCoachRequestRepository requestRepository,
    ICoachAssignmentService coachAssignmentService,
    IClientCoachRequestAccessService accessService
    ) : ICommandHandler<UpdateClientCoachRequestCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IClientCoachRequestRepository _requestRepository = requestRepository;
    private readonly ICoachAssignmentService _coachAssignmentService = coachAssignmentService;
    private readonly IClientCoachRequestAccessService _accessService = accessService;

    public async Task<Result> ExecuteAsync(UpdateClientCoachRequestCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Coach)
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var request = await _requestRepository.GetByIdAsync(command.RequstId, cancellationToken, track: true);
        if (request == null)
            return Result.Fail(DomainErrors.NotFound(nameof(ClientCoachRequest)));

        if (!_accessService.HasAccesToRequest(request, currentUser))
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        if (request.Status != CoachRequestStatus.Pending)
            return Result.Fail(ClientCoachRequestErrors.CannotUpdateFinalizedRequest, StatusCodes.Status409Conflict);

        request.Status = command.Status;
        request.ReviewedAt = DateTime.UtcNow;

        if (command.Status != CoachRequestStatus.Accepted)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        var clientUpdateResult = await _coachAssignmentService.AssignCoachToClientAsync(request.ClientId, request.CoachId, cancellationToken);
        if (!clientUpdateResult.IsSuccess)
            return Result.Fail(clientUpdateResult.Errors!, clientUpdateResult.StatusCode);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}