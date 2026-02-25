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

namespace FitCoachPro.Application.Queries.ClientCoachRequests.GetClientCoachRequestById;

public class GetClientCoachRequestByIdQueryHandler(
    IUserContextService userContext,
    IClientCoachRequestRepository requestRepository,
    IClientCoachRequestAccessService accessService
    ) : IQueryHandler<GetClientCoachRequestByIdQuery, Result<ClientCoachRequestModel>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IClientCoachRequestRepository _requestRepository = requestRepository;
    private readonly IClientCoachRequestAccessService _accessService = accessService;

    public async Task<Result<ClientCoachRequestModel>> ExecuteAsync(GetClientCoachRequestByIdQuery query, CancellationToken cancellationToken)
    {
        var request = await _requestRepository.GetByIdAsync(query.RequestId, cancellationToken);
        if (request == null)
            return Result<ClientCoachRequestModel>.Fail(DomainErrors.NotFound(nameof(ClientCoachRequest)));

        var currentUser = _userContext.Current;
        if (!_accessService.HasAccesToRequest(request, currentUser))
            return Result<ClientCoachRequestModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        return Result<ClientCoachRequestModel>.Success(request.ToModel());
    }
}
