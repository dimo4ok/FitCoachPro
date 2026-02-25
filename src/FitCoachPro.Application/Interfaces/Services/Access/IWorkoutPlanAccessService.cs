using FitCoachPro.Application.Common.Models;

namespace FitCoachPro.Application.Interfaces.Services.Access;

public interface IWorkoutPlanAccessService
{
    Task<bool> HasCoachAccessToWorkoutPlan(UserContext currentUser, Guid clientId, CancellationToken cancellationToken);
    Task<bool> HasUserAccessToWorkoutPlanAsync(UserContext currentUser, Guid clientId, CancellationToken cancellationToken);
}