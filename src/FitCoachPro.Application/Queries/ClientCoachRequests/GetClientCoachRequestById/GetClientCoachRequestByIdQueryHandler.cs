using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Models.Requests;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Queries.ClientCoachRequests.GetClientCoachRequestById;

public class GetClientCoachRequestByIdQueryHandler(
    IUserContextService userContext,
    IClientCoachRequestRepository requestRepository,
    IClientCoachRequestAccessService accessService,
    ILogger<GetClientCoachRequestByIdQueryHandler> logger
    ) : IQueryHandler<GetClientCoachRequestByIdQuery, Result<ClientCoachRequestModel>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IClientCoachRequestRepository _requestRepository = requestRepository;
    private readonly IClientCoachRequestAccessService _accessService = accessService;
    private readonly ILogger<GetClientCoachRequestByIdQueryHandler> _logger = logger;

    public async Task<Result<ClientCoachRequestModel>> ExecuteAsync(GetClientCoachRequestByIdQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        _logger.LogInformation(
            "GetClientCoachRequestById attempt started. RequestId: {RequestId}, UserId: {UserId}, Role: {Role}",
            query.RequestId, currentUser.UserId, currentUser.Role);

        var request = await _requestRepository.GetByIdAsync(query.RequestId, cancellationToken);
        if (request == null)
        {
            _logger.LogWarning(
                "GetClientCoachRequestById failed: Request not found. RequestId: {RequestId}",
                query.RequestId);
            return Result<ClientCoachRequestModel>.Fail(DomainErrors.NotFound(nameof(ClientCoachRequest)));
        }

        if (!_accessService.HasAccesToRequest(request, currentUser))
        {
            _logger.LogWarning(
                "GetClientCoachRequestById forbidden. RequestId: {RequestId}, UserId: {UserId}, Role: {Role}",
                query.RequestId, currentUser.UserId, currentUser.Role);
            return Result<ClientCoachRequestModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        _logger.LogInformation(
            "GetClientCoachRequestById succeeded. RequestId: {RequestId}, Status: {Status}",
            request.Id, request.Status);

        return Result<ClientCoachRequestModel>.Success(request.ToModel());
    }
}
