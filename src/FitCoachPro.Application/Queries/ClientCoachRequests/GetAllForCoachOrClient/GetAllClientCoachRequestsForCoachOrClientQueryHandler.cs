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
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Queries.ClientCoachRequests.GetAllClientCoachRequestsForCoachOrClient;

public class GetAllClientCoachRequestsForCoachOrClientQueryHandler(
    IUserContextService userContext,
    IClientCoachRequestRepository requestRepository,
    ILogger<GetAllClientCoachRequestsForCoachOrClientQueryHandler> logger
    ) : IQueryHandler<GetAllClientCoachRequestsForCoachOrClientQuery, Result<PaginatedModel<ClientCoachRequestModel>>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IClientCoachRequestRepository _requestRepository = requestRepository;
    private readonly ILogger<GetAllClientCoachRequestsForCoachOrClientQueryHandler> _logger = logger;

    public async Task<Result<PaginatedModel<ClientCoachRequestModel>>> ExecuteAsync(GetAllClientCoachRequestsForCoachOrClientQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        if (currentUser.Role != UserRole.Coach && currentUser.Role != UserRole.Client)
        {
            _logger.LogWarning(
                "GetAllClientCoachRequestsForCoachOrClient forbidden. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result<PaginatedModel<ClientCoachRequestModel>>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        _logger.LogInformation(
            "GetAllClientCoachRequestsForCoachOrClient attempt started. UserId: {UserId}, Role: {Role}, StatusFilter: {Status}, Page: {PageNumber}, Size: {PageSize}",
            currentUser.UserId, currentUser.Role, query.Status, query.PaginationParams.PageNumber, query.PaginationParams.PageSize);

        var requestsQuery = _requestRepository.GetAllByUserIdAndUserRoleAsQuery(currentUser.UserId, currentUser.Role);
        if (query.Status != null)
            requestsQuery = requestsQuery.Where(x => x.Status == query.Status);

        if (!await requestsQuery.AnyAsync(cancellationToken))
        {
            _logger.LogWarning(
                "GetAllClientCoachRequestsForCoachOrClient failed: No requests found. UserId: {UserId}, Role: {Role}, StatusFilter: {Status}",
                currentUser.UserId, currentUser.Role, query.Status);
            return Result<PaginatedModel<ClientCoachRequestModel>>.Fail(DomainErrors.NotFound(nameof(ClientCoachRequest)));
        }

        var paginated = await requestsQuery.PaginateAsync(query.PaginationParams.PageNumber, query.PaginationParams.PageSize, cancellationToken);

        _logger.LogInformation(
            "GetAllClientCoachRequestsForCoachOrClient succeeded. UserId: {UserId}, Role: {Role}, ReturnedCount: {Count}, Total: {Total}",
            currentUser.UserId, currentUser.Role, paginated.Items.Count, paginated.TotalItems);

        return Result<PaginatedModel<ClientCoachRequestModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }
}
