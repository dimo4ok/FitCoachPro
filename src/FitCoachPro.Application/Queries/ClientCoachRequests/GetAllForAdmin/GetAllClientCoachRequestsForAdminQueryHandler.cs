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

namespace FitCoachPro.Application.Queries.ClientCoachRequests.GetAllForAdmin;

public class GetAllClientCoachRequestsForAdminQueryHandler(
    IUserContextService userContext,
    IClientCoachRequestRepository requestRepository,
    ILogger<GetAllClientCoachRequestsForAdminQueryHandler> logger
    ) : IQueryHandler<GetAllClientCoachRequestsForAdminQuery, Result<PaginatedModel<ClientCoachRequestModel>>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IClientCoachRequestRepository _requestRepository = requestRepository;
    private readonly ILogger<GetAllClientCoachRequestsForAdminQueryHandler> _logger = logger;

    public async Task<Result<PaginatedModel<ClientCoachRequestModel>>> ExecuteAsync(GetAllClientCoachRequestsForAdminQuery command, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        _logger.LogInformation(
            "GetAllClientCoachRequestsForAdmin attempt started. AdminId: {AdminId}, UserIdFilter: {UserId}, StatusFilter: {Status}, Page: {PageNumber}, Size: {PageSize}",
            currentUser.UserId, command.UserId, command.Status, command.PaginationParams.PageNumber, command.PaginationParams.PageSize);

        if (currentUser.Role != UserRole.Admin)
        {
            _logger.LogWarning(
                "GetAllClientCoachRequestsForAdmin forbidden: User is not an Admin. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result<PaginatedModel<ClientCoachRequestModel>>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        var query = _requestRepository.GetAllByUserIdAsQuery(command.UserId);
        if (command.Status != null)
            query = query.Where(x => x.Status == command.Status);

        if (!await query.AnyAsync(cancellationToken))
        {
            _logger.LogWarning(
                "GetAllClientCoachRequestsForAdmin failed: No requests found. UserIdFilter: {UserId}, StatusFilter: {Status}",
                command.UserId, command.Status);
            return Result<PaginatedModel<ClientCoachRequestModel>>.Fail(DomainErrors.NotFound(nameof(ClientCoachRequest)));
        }

        var paginated = await query.PaginateAsync(command.PaginationParams.PageNumber, command.PaginationParams.PageSize, cancellationToken);

        _logger.LogInformation(
            "GetAllClientCoachRequestsForAdmin succeeded. ReturnedCount: {Count}, Total: {Total}",
            paginated.Items.Count, paginated.TotalItems);

        return Result<PaginatedModel<ClientCoachRequestModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }
}
