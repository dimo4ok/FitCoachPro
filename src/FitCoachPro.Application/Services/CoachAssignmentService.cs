using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.AspNetCore.Http;

namespace FitCoachPro.Application.Services;

public class CoachAssignmentService(IUserRepository userRepository) : ICoachAssignmentService
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<Result> AssignCoachToClientAsync(Guid clientId, Guid coachId, CancellationToken cancellationToken = default)
    {
        var client = await _userRepository.GetClientByIdAsync(clientId, cancellationToken, track: true);
        if (client == null)
            return Result.Fail(DomainErrors.NotFound(nameof(Client)));

        if (client.CoachId != null)
            return Result.Fail(ClientCoachRequestErrors.ClientAlreadyHasCoach, StatusCodes.Status409Conflict);

        var coach = await _userRepository.GetCoachByIdAsync(coachId, cancellationToken, track: true);
        if (coach == null)
            return Result.Fail(DomainErrors.NotFound(nameof(Coach)));

        if (coach.AcceptanceStatus == ClientAcceptanceStatus.NotAccepting)
            return Result.Fail(ClientCoachRequestErrors.CoachNotAcceptingNewClients, StatusCodes.Status409Conflict);

        client.CoachId = coachId;
        coach.Clients.Add(client);

        return Result.Success();
    }

    public async Task<Result> UnassignCoachAsync(Guid clientId, CancellationToken cancellationToken = default)
    {
        var client = await _userRepository.GetClientByIdWithCoachAsync(clientId, cancellationToken, true);
        if (client is null)
            return Result.Fail(DomainErrors.NotFound(nameof(Client)), StatusCodes.Status404NotFound);

        if (client.Coach is null)
            return Result.Fail(UserErrors.RelationshipNotFound(nameof(Client), nameof(Coach)), StatusCodes.Status400BadRequest);

        if (client.SubscriptionExpiresAt.HasValue &&
            client.SubscriptionExpiresAt.Value.Date > DateTime.UtcNow)
            return Result.Fail(UserErrors.ActiveSubscriptionPreventsUnassign, StatusCodes.Status400BadRequest);

        client.CoachId = null;
        client.Coach.Clients.Remove(client);

        return Result.Success();
    }
}
