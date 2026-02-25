using FitCoachPro.Application.Common.Models;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Services.Access;

public class WorkoutPlanAccessService(
    IUserRepository userRepository) : IWorkoutPlanAccessService
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<bool> HasUserAccessToWorkoutPlanAsync(UserContext currentUser, Guid clientId, CancellationToken cancellationToken) =>
       currentUser.Role switch
       {
           UserRole.Client => clientId == currentUser.UserId,
           UserRole.Coach => await _userRepository.CanCoachAccessClientAsync(currentUser.UserId, clientId, cancellationToken),
           UserRole.Admin => true,
           _ => false
       };

    public async Task<bool> HasCoachAccessToWorkoutPlan(UserContext currentUser, Guid clientId, CancellationToken cancellationToken) =>
        currentUser.Role switch
        {
            UserRole.Coach => await _userRepository.CanCoachAccessClientAsync(currentUser.UserId, clientId, cancellationToken),
            _ => false
        };
}
