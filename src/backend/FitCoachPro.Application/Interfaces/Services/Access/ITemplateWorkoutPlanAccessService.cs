using FitCoachPro.Application.Common.Models;

namespace FitCoachPro.Application.Interfaces.Services.Access;

public interface ITemplateWorkoutPlanAccessService
{
    Task<bool> HasUserAccessToTemplateAsync(Guid tempalteCoachId, UserContext currentUser);
}