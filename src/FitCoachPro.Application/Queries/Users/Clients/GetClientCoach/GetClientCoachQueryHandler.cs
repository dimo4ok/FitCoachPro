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

namespace FitCoachPro.Application.Queries.Users.Clients.GetClientCoach;

public class GetClientCoachQueryHandler(
    IUserContextService userContext,
    IUserRepository userRepository,
    ILogger<GetClientCoachQueryHandler> logger
    ) : IQueryHandler<GetClientCoachQuery, Result<CoachPrivateProfileModel>>
{
    private readonly IUserContextService _userContext = userContext;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILogger<GetClientCoachQueryHandler> _logger = logger;

    public async Task<Result<CoachPrivateProfileModel>> ExecuteAsync(GetClientCoachQuery query, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.Current;
        if (currentUser.Role != UserRole.Client)
        {
            _logger.LogWarning(
                "GetClientCoach forbidden: User is not a Client. UserId: {UserId}, Role: {Role}",
                currentUser.UserId, currentUser.Role);
            return Result<CoachPrivateProfileModel>.Fail(DomainErrors.Forbidden, StatusCodes.Status403Forbidden);
        }

        _logger.LogInformation(
            "GetClientCoach attempt started. ClientId: {ClientId}",
            currentUser.UserId);

        var client = await _userRepository.GetClientByIdAsync(currentUser.UserId, cancellationToken);
        if (client is null)
        {
            _logger.LogWarning(
                "GetClientCoach failed: Client not found. ClientId: {ClientId}",
                currentUser.UserId);
            return Result<CoachPrivateProfileModel>.Fail(DomainErrors.NotFound(nameof(Client)), StatusCodes.Status404NotFound);
        }

        if (!client.CoachId.HasValue)
        {
            _logger.LogWarning(
                "GetClientCoach failed: Client has no assigned coach. ClientId: {ClientId}",
                currentUser.UserId);
            return Result<CoachPrivateProfileModel>.Fail(UserErrors.RelationshipNotFound(nameof(Client), nameof(Coach)), StatusCodes.Status404NotFound);
        }

        var coach = await _userRepository.GetCoachByIdAsync(client.CoachId.Value, cancellationToken);
        if (coach is null)
        {
            _logger.LogWarning(
                "GetClientCoach failed: Coach not found. CoachId: {CoachId}, ClientId: {ClientId}",
                client.CoachId.Value, currentUser.UserId);
            return Result<CoachPrivateProfileModel>.Fail(DomainErrors.NotFound(nameof(Coach)), StatusCodes.Status404NotFound);
        }

        _logger.LogInformation(
            "GetClientCoach succeeded. ClientId: {ClientId}, CoachId: {CoachId}",
            currentUser.UserId, coach.Id);

        return Result<CoachPrivateProfileModel>.Success(coach.ToPrivateModel());
    }
}
