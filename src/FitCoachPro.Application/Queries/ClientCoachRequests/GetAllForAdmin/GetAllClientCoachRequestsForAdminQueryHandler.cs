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

namespace FitCoachPro.Application.Queries.ClientCoachRequests.GetAllForAdmin;

public class GetAllClientCoachRequestsForAdminQueryHandler(
    IUserContextService userContext,
    IClientCoachRequestRepository requestRepository
    ) : IQueryHandler<GetAllClientCoachRequestsForAdminQuery, Result<PaginatedModel<ClientCoachRequestModel>>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IClientCoachRequestRepository _requestRepository = requestRepository;

    public async Task<Result<PaginatedModel<ClientCoachRequestModel>>> ExecuteAsync(GetAllClientCoachRequestsForAdminQuery command, CancellationToken cancellationToken)
    {
        if (_userContext.Current.Role != UserRole.Admin)
            return Result<PaginatedModel<ClientCoachRequestModel>>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var query = _requestRepository.GetAllByUserIdAsQuery(command.UserId);
        if (command.Status != null)
            query = query.Where(x => x.Status == command.Status);

        if (!await query.AnyAsync(cancellationToken))
            return Result<PaginatedModel<ClientCoachRequestModel>>.Fail(DomainErrors.NotFound(nameof(ClientCoachRequest)));

        var paginated = await query.PaginateAsync(command.PaginationParams.PageNumber, command.PaginationParams.PageSize, cancellationToken);

        return Result<PaginatedModel<ClientCoachRequestModel>>.Success(paginated.ToModel(x => x.ToModel()));
    }
}
