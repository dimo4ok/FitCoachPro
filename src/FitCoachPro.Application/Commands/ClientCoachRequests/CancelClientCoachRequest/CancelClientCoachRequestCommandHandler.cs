using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities;
using FitCoachPro.Domain.Entities.Enums;
using Microsoft.AspNetCore.Http;

namespace FitCoachPro.Application.Commands.ClientCoachRequests.CancelClientCoachRequest;

public class CancelClientCoachRequestCommandHandler(
    IUserContextService userContext,
    IUnitOfWork unitOfWork,
    IClientCoachRequestRepository requestRepository,
    IClientCoachRequestAccessService accessService
    ) : ICommandHandler<CancelClientCoachRequestCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IClientCoachRequestRepository _requestRepository = requestRepository;
    private readonly IClientCoachRequestAccessService _accessService = accessService;

    public async Task<Result> ExecuteAsync(CancelClientCoachRequestCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Client)
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var request = await _requestRepository.GetByIdAsync(command.RequestId, cancellationToken, track: true);
        if (request == null)
            return Result.Fail(DomainErrors.NotFound(nameof(ClientCoachRequest)));

        if (!_accessService.HasAccesToRequest(request, currentUser))
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        if (request.Status != CoachRequestStatus.Pending)
            return Result.Fail(ClientCoachRequestErrors.CannotUpdateFinalizedRequest, StatusCodes.Status409Conflict);

        _requestRepository.Delete(request);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(StatusCodes.Status204NoContent);
    }
}
