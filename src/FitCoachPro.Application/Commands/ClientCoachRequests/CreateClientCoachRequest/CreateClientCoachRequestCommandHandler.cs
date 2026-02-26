using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities;
using FitCoachPro.Domain.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Commands.ClientCoachRequests.CreateClientCoachRequest;

public class CreateClientCoachRequestCommandHandler(
    IUserContextService userContext,
    IUnitOfWork unitOfWork,
    IClientCoachRequestRepository requestRepository,
    ILogger<CreateClientCoachRequestCommandHandler> logger
    ) : ICommandHandler<CreateClientCoachRequestCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IClientCoachRequestRepository _requestRepository = requestRepository;
    private readonly ILogger<CreateClientCoachRequestCommandHandler> _logger = logger;

    public async Task<Result> ExecuteAsync(CreateClientCoachRequestCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        _logger.LogInformation(
            "CreateClientCoachRequest attempt started. ClientId: {ClientId}, CoachId: {CoachId}",
            currentUser.UserId, command.CoachId);

        if (currentUser.Role != UserRole.Client)
        {
            _logger.LogWarning(
                "CreateClientCoachRequest forbidden: User is not a Client. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        if (!await _requestRepository.IsCoachAcceptingNewClientsAsync(command.CoachId, cancellationToken))
        {
            _logger.LogWarning(
                "CreateClientCoachRequest failed: Coach is not accepting new clients. ClientId: {ClientId}, CoachId: {CoachId}",
                currentUser.UserId, command.CoachId);
            return Result.Fail(ClientCoachRequestErrors.CoachNotAcceptingNewClients, StatusCodes.Status409Conflict);
        }

        if (!await _requestRepository.IsClientAvailableForNewCoachAsync(currentUser.UserId, cancellationToken))
        {
            _logger.LogWarning(
                "CreateClientCoachRequest failed: Client already has a coach. ClientId: {ClientId}",
                currentUser.UserId);
            return Result.Fail(ClientCoachRequestErrors.ClientAlreadyHasCoach, StatusCodes.Status409Conflict);
        }

        if (await _requestRepository.IsDuplicateRequestAsync(currentUser.UserId, command.CoachId, cancellationToken))
        {
            _logger.LogWarning(
                "CreateClientCoachRequest failed: Duplicate pending request already exists. ClientId: {ClientId}, CoachId: {CoachId}",
                currentUser.UserId, command.CoachId);
            return Result.Fail(ClientCoachRequestErrors.PendingRequestAlreadyExists, StatusCodes.Status409Conflict);
        }

        var request = new ClientCoachRequest
        {
            ClientId = currentUser.UserId,
            CoachId = command.CoachId,
            Status = CoachRequestStatus.Pending
        };

        await _requestRepository.CreateAsync(request, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "CreateClientCoachRequest succeeded. RequestId: {RequestId}, ClientId: {ClientId}, CoachId: {CoachId}",
            request.Id, request.ClientId, request.CoachId);

        return Result.Success(StatusCodes.Status201Created);
    }
}
