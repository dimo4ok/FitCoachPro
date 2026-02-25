using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Requests;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities;
using FitCoachPro.Domain.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Application.Queries.ClientCoachRequests.GetAllClientCoachRequestsForCoachOrClient;

public class GetAllClientCoachRequestsForCoachOrClientQueryHandler(
    IUserContextService userContext,
    IClientCoachRequestRepository requestRepository
    ) : IQueryHandler<GetAllClientCoachRequestsForCoachOrClientQuery, Result<PaginatedModel<ClientCoachRequestModel>>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IClientCoachRequestRepository _requestRepository = requestRepository;

    public async Task<Result<PaginatedModel<ClientCoachRequestModel>>> ExecuteAsync(GetAllClientCoachRequestsForCoachOrClientQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        if (currentUser.Role != UserRole.Coach && currentUser.Role != UserRole.Client)
            return Result<PaginatedModel<ClientCoachRequestModel>>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var requestsQuery = _requestRepository.GetAllByUserIdAndUserRoleAsQuery(currentUser.UserId, currentUser.Role);
        if (query.Status != null)
            requestsQuery = requestsQuery.Where(x => x.Status == query.Status);

        if (!await requestsQuery.AnyAsync(cancellationToken))
            return Result<PaginatedModel<ClientCoachRequestModel>>.Fail(DomainErrors.NotFound(nameof(ClientCoachRequest)));

        var paginated = await requestsQuery.PaginateAsync(query.PaginationParams.PageNumber, query.PaginationParams.PageSize, cancellationToken);

        return Result<PaginatedModel<ClientCoachRequestModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }
}
