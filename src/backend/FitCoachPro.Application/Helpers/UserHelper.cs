using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Helpers;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Domain.Entities;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Identity;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Application.Helpers;

public class UserHelper(
    UserManager<User> userManager,
    IUserRepository userRepository,
    IClientCoachRequestRepository clientCoachRequestRepository
    ) : IUserHelper
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IClientCoachRequestRepository _clientCoachRequestRepository = clientCoachRequestRepository;

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

    public async Task<Result> UpdateEmailAsync(User user, string newEmail)
    {
        if(user.Email == newEmail)
            return Result.Success();

        var existingEmail = await _userManager.FindByEmailAsync(newEmail);
        if (existingEmail != null)
            return Result.Fail(UserErrors.EmailAlreadyExists, StatusCodes.Status400BadRequest);

        var setEmailResult = await _userManager.SetEmailAsync(user, newEmail);
        if (!setEmailResult.Succeeded)
            return Result.Fail(setEmailResult.Errors.ToErrorList(), StatusCodes.Status400BadRequest);

        return Result.Success();
    }

    public async Task<Result> UpdatePhoneNumberAsync(User user, string? newPhoneNumber)
    {
        if (user.PhoneNumber == newPhoneNumber)
            return Result.Success();

        var existingPhone = await _userManager.Users.AnyAsync(x => x.PhoneNumber == newPhoneNumber);
        if (existingPhone)
            return Result.Fail(UserErrors.PhoneAlreadyExists, StatusCodes.Status400BadRequest);

        var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, newPhoneNumber);
        if (!setPhoneResult.Succeeded)
            return Result.Fail(setPhoneResult.Errors.ToErrorList(), StatusCodes.Status400BadRequest);

        return Result.Success();
    }

    public async Task<Result> UpdatePasswordAsync(User user, UpdatePasswordModel model)
    {
        if(model.NewPassword != model.ConfirmPassword)
            return Result.Fail(UserErrors.PasswordsDoNotMatch, StatusCodes.Status400BadRequest);

        var passwordChangeResult = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);
        if (!passwordChangeResult.Succeeded)
            return Result.Fail(passwordChangeResult.Errors.ToErrorList(), StatusCodes.Status400BadRequest);

        return Result.Success();
    }

    public async Task<Result> DeleteAccountAsync(UserProfile userProfile, CancellationToken cancellationToken)
    {
        List<ClientCoachRequest> requests;

        switch (userProfile)
        {
            case Client:
                requests = await _clientCoachRequestRepository
                    .GetAllByUserIdAndUserRoleAsQuery(userProfile.Id, UserRole.Client, true)
                    .ToListAsync(cancellationToken);
                break;

            case Coach:
                requests = await _clientCoachRequestRepository
                    .GetAllByUserIdAndUserRoleAsQuery(userProfile.Id, UserRole.Coach, true)
                    .ToListAsync(cancellationToken);
                break;

            default:
                return Result.Fail(DomainErrors.Forbidden);
        }

        if (requests.Count > 0)
            _clientCoachRequestRepository.DeleteRequests(requests);

        _userRepository.Delete(userProfile.User);

        return Result.Success();
    }
}