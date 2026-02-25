using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities;
using FitCoachPro.Domain.Entities.Enums;
using Microsoft.AspNetCore.Http;

namespace FitCoachPro.Application.Commands.ClientCoachRequests.CreateClientCoachRequest;

public class CreateClientCoachRequestCommandHandler(
    IUserContextService userContext,
    IUnitOfWork unitOfWork,
    IClientCoachRequestRepository requestRepository
    ) : ICommandHandler<CreateClientCoachRequestCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IClientCoachRequestRepository _requestRepository = requestRepository;

    public async Task<Result> ExecuteAsync(CreateClientCoachRequestCommand command, CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.Current.UserId;

        if (_userContext.Current.Role != UserRole.Client)
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        if (!await _requestRepository.IsCoachAcceptingNewClientsAsync(command.CoachId, cancellationToken))
            return Result.Fail(ClientCoachRequestErrors.CoachNotAcceptingNewClients, StatusCodes.Status409Conflict);

        if (!await _requestRepository.IsClientAvailableForNewCoachAsync(currentUserId, cancellationToken))
            return Result.Fail(ClientCoachRequestErrors.ClientAlreadyHasCoach, StatusCodes.Status409Conflict);

        if (await _requestRepository.IsDuplicateRequestAsync(currentUserId, command.CoachId, cancellationToken))
            return Result.Fail(ClientCoachRequestErrors.PendingRequestAlreadyExists, StatusCodes.Status409Conflict);

        var request = new ClientCoachRequest
        {
            ClientId = currentUserId,
            CoachId = command.CoachId,
            Status = CoachRequestStatus.Pending
        };

        await _requestRepository.CreateAsync(request, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(StatusCodes.Status201Created);
    }
}
