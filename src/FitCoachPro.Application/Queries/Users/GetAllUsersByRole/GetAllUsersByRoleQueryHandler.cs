using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Models.Pagination;
using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Application.Queries.Users.GetAllUsersByRole;

public class GetAllUsersByRoleQueryHandler(
    IUserContextService userContext,
    IUserRepository userRepository,
    IUsersAccessService accessService
    ) : IQueryHandler<GetAllUsersByRoleQuery, Result<PaginatedModel<UserProfileModel>>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUsersAccessService _accessService = accessService;

    public async Task<Result<PaginatedModel<UserProfileModel>>> ExecuteAsync(GetAllUsersByRoleQuery query, CancellationToken cancellationToken)
    {
        if (!_accessService.HasCurrentUserAccessToUsers(_userContext.Current.Role, query.Role))
            return Result<PaginatedModel<UserProfileModel>>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var usersQuery = _userRepository.GetAllUsersByRoleAsQuery(query.Role);
        if (!await usersQuery.AnyAsync(cancellationToken))
            return Result<PaginatedModel<UserProfileModel>>.Fail(DomainErrors.NotFound(nameof(UserProfile)), StatusCodes.Status404NotFound);

        var paginated = await usersQuery.PaginateAsync(query.PaginationParams.PageNumber, query.PaginationParams.PageSize, cancellationToken);

        return Result<PaginatedModel<UserProfileModel>>.Success(paginated.ToModel(x => x!.ToModel()));
    }
}