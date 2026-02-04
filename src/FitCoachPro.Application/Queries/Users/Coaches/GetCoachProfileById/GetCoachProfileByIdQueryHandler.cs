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

namespace FitCoachPro.Application.Queries.Users.Coaches.GetCoachProfileById;

public class GetCoachProfileByIdQueryHandler(
    IUserContextService userContext,
    IUserRepository userRepository,
    IUsersAccessService accessService
    )
    : IQueryHandler<GetCoachProfileByIdQuery, Result<CoachPublicProfileModel>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUsersAccessService _accessService = accessService;

    public async Task<Result<CoachPublicProfileModel>> ExecuteAsync(GetCoachProfileByIdQuery query, CancellationToken cancellationToken)
    {
        if (!_accessService.HasCurrentUserAccessToUsers(_userContext.Current.Role, UserRole.Coach))
            return Result<CoachPublicProfileModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var coach = await _userRepository.GetCoachByIdAsync(query.Id, cancellationToken);
        if (coach is null)
            return Result<CoachPublicProfileModel>.Fail(DomainErrors.NotFound(nameof(Coach)), StatusCodes.Status404NotFound);

        return Result<CoachPublicProfileModel>.Success(coach.ToPublicModel());
    }
}
