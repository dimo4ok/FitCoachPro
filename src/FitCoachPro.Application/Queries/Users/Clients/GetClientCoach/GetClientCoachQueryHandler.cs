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

namespace FitCoachPro.Application.Queries.Users.Clients.GetClientCoach;

public class GetClientCoachQueryHandler(
    IUserContextService userContext,
    IUserRepository userRepository
    ) : IQueryHandler<GetClientCoachQuery, Result<CoachPrivateProfileModel>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result<CoachPrivateProfileModel>> ExecuteAsync(GetClientCoachQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Client)
            return Result<CoachPrivateProfileModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);

        var client = await _userRepository.GetClientByIdAsync(currentUser.UserId, cancellationToken);
        if (client is null)
            return Result<CoachPrivateProfileModel>.Fail(DomainErrors.NotFound(nameof(Client)), StatusCodes.Status404NotFound);

        if (!client.CoachId.HasValue)
            return Result<CoachPrivateProfileModel>.Fail(UserErrors.RelationshipNotFound(nameof(Client), nameof(Coach)), StatusCodes.Status404NotFound);

        var coach = await _userRepository.GetCoachByIdAsync(client.CoachId.Value, cancellationToken);
        if (coach is null)
            return Result<CoachPrivateProfileModel>.Fail(DomainErrors.NotFound(nameof(Coach)), StatusCodes.Status404NotFound);

        return Result<CoachPrivateProfileModel>.Success(coach.ToPrivateModel());
    }
}
