using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities;
using FitCoachPro.Domain.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Commands.ClientCoachRequests.DeleteOwnClientCoachRequests;

public class DeleteOwnClientCoachRequestsCommandHandler(
    IUserContextService userContext,
    IUnitOfWork unitOfWork,
    IClientCoachRequestRepository requestRepository,
    ILogger<DeleteOwnClientCoachRequestsCommandHandler> logger
    ) : ICommandHandler<DeleteOwnClientCoachRequestsCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IClientCoachRequestRepository _requestRepository = requestRepository;
    private readonly ILogger<DeleteOwnClientCoachRequestsCommandHandler> _logger = logger;

    public async Task<Result> ExecuteAsync(DeleteOwnClientCoachRequestsCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        _logger.LogInformation(
            "DeleteOwnClientCoachRequests attempt started. UserId: {UserId}, Role: {Role}",
            currentUser.UserId, currentUser.Role);

        if (currentUser.Role != UserRole.Coach && currentUser.Role != UserRole.Client)
        {
            _logger.LogWarning(
                "DeleteOwnClientCoachRequests forbidden. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        var requests = await _requestRepository
            .GetAllByUserIdAndUserRoleAsQuery(currentUser.UserId, currentUser.Role, track: true)
            .ToListAsync(cancellationToken);
        if (requests.Count == 0)
        {
            _logger.LogWarning(
                "DeleteOwnClientCoachRequests failed: No requests found. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result.Fail(DomainErrors.NotFound(nameof(ClientCoachRequest)));
        }

        _requestRepository.DeleteRequests(requests);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "DeleteOwnClientCoachRequests succeeded. UserId: {UserId}, Role: {Role}, DeletedCount: {Count}",
            currentUser.UserId, currentUser.Role, requests.Count);

        return Result.Success(StatusCodes.Status204NoContent);
    }
}
