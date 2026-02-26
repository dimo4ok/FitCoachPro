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
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Queries.Users.GetAllUsersByRole;

public class GetAllUsersByRoleQueryHandler(
    IUserContextService userContext,
    IUserRepository userRepository,
    IUsersAccessService accessService,
    ILogger<GetAllUsersByRoleQueryHandler> logger
    ) : IQueryHandler<GetAllUsersByRoleQuery, Result<PaginatedModel<UserProfileModel>>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUsersAccessService _accessService = accessService;
    private readonly ILogger<GetAllUsersByRoleQueryHandler> _logger = logger;

    public async Task<Result<PaginatedModel<UserProfileModel>>> ExecuteAsync(GetAllUsersByRoleQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        _logger.LogInformation(
            "GetAllUsersByRole attempt started. RequestedRole: {RequestedRole}, CurrentUserId: {UserId}, CurrentUserRole: {Role}, Page: {PageNumber}, Size: {PageSize}",
            query.Role, currentUser.UserId, currentUser.Role, query.PaginationParams.PageNumber, query.PaginationParams.PageSize);

        if (!_accessService.HasCurrentUserAccessToUsers(currentUser.Role, query.Role))
        {
            _logger.LogWarning(
                "GetAllUsersByRole forbidden. RequestedRole: {RequestedRole}, CurrentUserId: {UserId}, CurrentUserRole: {Role}",
                query.Role, currentUser.UserId, currentUser.Role);
                
            return Result<PaginatedModel<UserProfileModel>>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        var usersQuery = _userRepository.GetAllUsersByRoleAsQuery(query.Role);
        if (!await usersQuery.AnyAsync(cancellationToken))
        {
            _logger.LogWarning(
                "GetAllUsersByRole failed: No users found. RequestedRole: {RequestedRole}",
                query.Role);
            return Result<PaginatedModel<UserProfileModel>>.Fail(DomainErrors.NotFound(nameof(UserProfile)), StatusCodes.Status404NotFound);
        }

        var paginated = await usersQuery.PaginateAsync(query.PaginationParams.PageNumber, query.PaginationParams.PageSize, cancellationToken);

        _logger.LogInformation(
            "GetAllUsersByRole succeeded. RequestedRole: {RequestedRole}, ReturnedCount: {Count}, Total: {Total}",
            query.Role, paginated.Items.Count, paginated.TotalItems);

        return Result<PaginatedModel<UserProfileModel>>.Success(paginated.ToModel(x => x!.ToModel()));
    }
}