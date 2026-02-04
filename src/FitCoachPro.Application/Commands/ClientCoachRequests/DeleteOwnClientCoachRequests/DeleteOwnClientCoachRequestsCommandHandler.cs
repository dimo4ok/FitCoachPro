using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities;
using FitCoachPro.Domain.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Application.Commands.ClientCoachRequests.DeleteOwnClientCoachRequests;

public class DeleteOwnClientCoachRequestsCommandHandler(
    IUserContextService userContext,
    IUnitOfWork unitOfWork,
    IClientCoachRequestRepository requestRepository
    ) : ICommandHandler<DeleteOwnClientCoachRequestsCommand, Result>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IClientCoachRequestRepository _requestRepository = requestRepository;

    public async Task<Result> ExecuteAsync(DeleteOwnClientCoachRequestsCommand command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Coach && currentUser.Role != UserRole.Client)
            return Result.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var requests = await _requestRepository
            .GetAllByUserIdAndUserRoleAsQuery(currentUser.UserId, currentUser.Role, track: true)
            .ToListAsync(cancellationToken);
        if (requests.Count == 0)
            return Result.Fail(DomainErrors.NotFound(nameof(ClientCoachRequest)));

        _requestRepository.DeleteRequests(requests);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(StatusCodes.Status204NoContent);
    }
}
