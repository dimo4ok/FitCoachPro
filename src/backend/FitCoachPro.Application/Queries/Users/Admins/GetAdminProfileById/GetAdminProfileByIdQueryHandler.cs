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

namespace FitCoachPro.Application.Queries.Users.Admins.GetAdminProfileById;

public class GetAdminProfileByIdQueryHandler(
    IUserContextService userContext,
    IUserRepository userRepository,
    IUsersAccessService accessService
    ) : IQueryHandler<GetAdminProfileByIdQuery, Result<AdminPublicProfileModel>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUsersAccessService _accessService = accessService;

    public async Task<Result<AdminPublicProfileModel>> ExecuteAsync(GetAdminProfileByIdQuery query, CancellationToken cancellationToken)
    {
        if (!_accessService.HasCurrentUserAccessToUsers(_userContext.Current.Role, UserRole.Admin))
            return Result<AdminPublicProfileModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var admin = await _userRepository.GetAdminByIdAsync(query.Id, cancellationToken);
        if (admin is null)
            return Result<AdminPublicProfileModel>.Fail(DomainErrors.NotFound(nameof(Admin)), StatusCodes.Status404NotFound);

        return Result<AdminPublicProfileModel>.Success(admin.ToPublicModel());
    }
}
