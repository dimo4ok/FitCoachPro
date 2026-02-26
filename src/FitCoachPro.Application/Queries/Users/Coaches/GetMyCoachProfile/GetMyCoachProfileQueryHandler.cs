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

namespace FitCoachPro.Application.Queries.Users.Coaches.GetMyCoachProfile;

public class GetMyCoachProfileQueryHandler(
    IUserRepository userRepository,
    IUserContextService userContext,
    ILogger<GetMyCoachProfileQueryHandler> logger
    ) : IQueryHandler<GetMyCoachProfileQuery, Result<CoachPrivateProfileModel>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserContextService _userContext = userContext;
    private readonly ILogger<GetMyCoachProfileQueryHandler> _logger = logger;

    public async Task<Result<CoachPrivateProfileModel>> ExecuteAsync(GetMyCoachProfileQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Coach)
        {
            _logger.LogWarning(
                "GetMyCoachProfile forbidden: User is not a Coach. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result<CoachPrivateProfileModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        _logger.LogInformation(
            "GetMyCoachProfile attempt started. CoachId: {CoachId}",
            currentUser.UserId);

        var coach = await _userRepository.GetCoachByIdAsync(currentUser.UserId, cancellationToken);
        if (coach is null)
        {
            _logger.LogWarning(
                "GetMyCoachProfile failed: Coach not found. CoachId: {CoachId}",
                currentUser.UserId);
            return Result<CoachPrivateProfileModel>.Fail(DomainErrors.NotFound(nameof(Coach)), StatusCodes.Status404NotFound);
        }

        _logger.LogInformation(
            "GetMyCoachProfile succeeded. CoachId: {CoachId}",
            currentUser.UserId);

        return Result<CoachPrivateProfileModel>.Success(coach.ToPrivateModel());
    }
}
