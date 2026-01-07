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

namespace FitCoachPro.Application.Queries.Users.Coaches.GetMyCoachProfile;

public class GetMyCoachProfileQueryHandler(
    IUserRepository userRepository,
    IUserContextService userContext
    ) : IQueryHandler<GetMyCoachProfileQuery, Result<CoachPrivateProfileModel>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserContextService _userContext = userContext;

    public async Task<Result<CoachPrivateProfileModel>> ExecuteAsync(GetMyCoachProfileQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Coach)
            return Result<CoachPrivateProfileModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var coach = await _userRepository.GetCoachByIdAsync(currentUser.UserId, cancellationToken);
        if (coach is null)
            return Result<CoachPrivateProfileModel>.Fail(DomainErrors.NotFound(nameof(Coach)), StatusCodes.Status404NotFound);

        return Result<CoachPrivateProfileModel>.Success(coach.ToPrivateModel());
    }
}
