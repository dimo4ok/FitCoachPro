using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Application.Queries.Users.Coaches.GetCoachClients;

public class GetCoachClientsQueryHandler(
    IUserContextService userContext,
    IUserRepository userRepository
    ) : IQueryHandler<GetCoachClientsQuery, Result<PaginatedModel<ClientPrivateProfileModel>>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result<PaginatedModel<ClientPrivateProfileModel>>> ExecuteAsync(GetCoachClientsQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Coach)
            return Result<PaginatedModel<ClientPrivateProfileModel>>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var clientsQuery = _userRepository.GetAllCoachClientsAsQuery(currentUser.UserId);
        if (!await clientsQuery.AnyAsync(cancellationToken))
            return Result<PaginatedModel<ClientPrivateProfileModel>>.Fail(DomainErrors.NotFound(nameof(Client)), StatusCodes.Status404NotFound);

        var paginated = await clientsQuery.PaginateAsync(query.PaginationParams.PageNumber, query.PaginationParams.PageSize,cancellationToken);

        return Result<PaginatedModel<ClientPrivateProfileModel>>.Success(paginated.ToModel(x => x.ToPrivateModel()));
    }
}