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
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Queries.Users.Admins.GetMyAdminProfile;

public class GetMyAdminProfileQueryHandler(
    IUserRepository userRepository,
    IUserContextService userContext,
    ILogger<GetMyAdminProfileQueryHandler> logger
    ) : IQueryHandler<GetMyAdminProfileQuery, Result<AdminPrivateProfileModel>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserContextService _userContext = userContext;
    private readonly ILogger<GetMyAdminProfileQueryHandler> _logger = logger;

    public async Task<Result<AdminPrivateProfileModel>> ExecuteAsync(GetMyAdminProfileQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if(currentUser.Role != UserRole.Admin)
        {
            _logger.LogWarning(
                "GetMyAdminProfile forbidden: User is not an Admin. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result<AdminPrivateProfileModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        _logger.LogInformation(
            "GetMyAdminProfile attempt started. AdminId: {AdminId}",
            currentUser.UserId);

        var admin = await _userRepository.GetAdminByIdAsync(currentUser.UserId, cancellationToken);
        if (admin is null)
        {
            _logger.LogWarning(
                "GetMyAdminProfile failed: Admin not found. AdminId: {AdminId}",
                currentUser.UserId);
            return Result<AdminPrivateProfileModel>.Fail(DomainErrors.NotFound(nameof(Admin)), StatusCodes.Status404NotFound);
        }

        _logger.LogInformation(
            "GetMyAdminProfile succeeded. AdminId: {AdminId}",
            currentUser.UserId);

        return Result<AdminPrivateProfileModel>.Success(admin.ToPrivateModel());
    }
}
