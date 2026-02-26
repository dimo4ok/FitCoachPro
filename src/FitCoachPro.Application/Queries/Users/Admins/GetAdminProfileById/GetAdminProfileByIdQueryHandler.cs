using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Queries.Users.Admins.GetAdminProfileById;

public class GetAdminProfileByIdQueryHandler(
    IUserContextService userContext,
    IUserRepository userRepository,
    IUsersAccessService accessService,
    ILogger<GetAdminProfileByIdQueryHandler> logger
    ) : IQueryHandler<GetAdminProfileByIdQuery, Result<AdminPublicProfileModel>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUsersAccessService _accessService = accessService;
    private readonly ILogger<GetAdminProfileByIdQueryHandler> _logger = logger;

    public async Task<Result<AdminPublicProfileModel>> ExecuteAsync(GetAdminProfileByIdQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        _logger.LogInformation(
            "GetAdminProfileById attempt started. AdminId: {AdminId}, CurrentUserId: {UserId}, CurrentUserRole: {Role}",
            query.Id, currentUser.UserId, currentUser.Role);

        if (!_accessService.HasCurrentUserAccessToUsers(currentUser.Role, UserRole.Admin))
        {
            _logger.LogWarning(
                "GetAdminProfileById forbidden. AdminId: {AdminId}, CurrentUserId: {UserId}, CurrentUserRole: {Role}",
                query.Id, currentUser.UserId, currentUser.Role);
            return Result<AdminPublicProfileModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        var admin = await _userRepository.GetAdminByIdAsync(query.Id, cancellationToken);
        if (admin is null)
        {
            _logger.LogWarning(
                "GetAdminProfileById failed: Admin not found. AdminId: {AdminId}",
                query.Id);
            return Result<AdminPublicProfileModel>.Fail(DomainErrors.NotFound(nameof(Admin)), StatusCodes.Status404NotFound);
        }

        _logger.LogInformation(
            "GetAdminProfileById succeeded. AdminId: {AdminId}",
            query.Id);

        return Result<AdminPublicProfileModel>.Success(admin.ToPublicModel());
    }
}
