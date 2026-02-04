using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.AspNetCore.Http;

namespace FitCoachPro.Application.Queries.Users.Admins.GetMyAdminProfile;

public class GetMyAdminProfileQueryHandler(
    IUserRepository userRepository,
    IUserContextService userContext
    ) : IQueryHandler<GetMyAdminProfileQuery, Result<AdminPrivateProfileModel>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserContextService _userContext = userContext;

    public async Task<Result<AdminPrivateProfileModel>> ExecuteAsync(GetMyAdminProfileQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if(currentUser.Role != UserRole.Admin)
            return Result<AdminPrivateProfileModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var admin = await _userRepository.GetAdminByIdAsync(currentUser.UserId, cancellationToken);
        if (admin is null)
            return Result<AdminPrivateProfileModel>.Fail(DomainErrors.NotFound(nameof(Admin)), StatusCodes.Status404NotFound);

        return Result<AdminPrivateProfileModel>.Success(admin.ToPrivateModel());
    }
}
