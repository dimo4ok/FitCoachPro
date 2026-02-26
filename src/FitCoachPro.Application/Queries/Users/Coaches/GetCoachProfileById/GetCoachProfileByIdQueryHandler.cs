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

namespace FitCoachPro.Application.Queries.Users.Coaches.GetCoachProfileById;

public class GetCoachProfileByIdQueryHandler(
    IUserContextService userContext,
    IUserRepository userRepository,
    IUsersAccessService accessService,
    ILogger<GetCoachProfileByIdQueryHandler> logger
    )
    : IQueryHandler<GetCoachProfileByIdQuery, Result<CoachPublicProfileModel>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUsersAccessService _accessService = accessService;
    private readonly ILogger<GetCoachProfileByIdQueryHandler> _logger = logger;

    public async Task<Result<CoachPublicProfileModel>> ExecuteAsync(GetCoachProfileByIdQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;

        _logger.LogInformation(
            "GetCoachProfileById attempt started. CoachId: {CoachId}, CurrentUserId: {UserId}, CurrentUserRole: {Role}",
            query.Id, currentUser.UserId, currentUser.Role);

        if (!_accessService.HasCurrentUserAccessToUsers(currentUser.Role, UserRole.Coach))
        {
            _logger.LogWarning(
                "GetCoachProfileById forbidden. CoachId: {CoachId}, CurrentUserId: {UserId}, CurrentUserRole: {Role}",
                query.Id, currentUser.UserId, currentUser.Role);
            return Result<CoachPublicProfileModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        var coach = await _userRepository.GetCoachByIdAsync(query.Id, cancellationToken);
        if (coach is null)
        {
            _logger.LogWarning(
                "GetCoachProfileById failed: Coach not found. CoachId: {CoachId}",
                query.Id);
            return Result<CoachPublicProfileModel>.Fail(DomainErrors.NotFound(nameof(Coach)), StatusCodes.Status404NotFound);
        }

        _logger.LogInformation(
            "GetCoachProfileById succeeded. CoachId: {CoachId}",
            query.Id);

        return Result<CoachPublicProfileModel>.Success(coach.ToPublicModel());
    }
}
