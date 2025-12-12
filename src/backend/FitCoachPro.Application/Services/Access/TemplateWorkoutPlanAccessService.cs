using FitCoachPro.Application.Common.Models;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.Application.Services.Access;

public class TemplateWorkoutPlanAccessService : ITemplateWorkoutPlanAccessService
{
    public async Task<bool> HasUserAccessToTemplateAsync(Guid tempalteCoachId, UserContext currentUser) =>
        currentUser.Role switch
        {
            UserRole.Admin => true,
            UserRole.Coach => currentUser.UserId == tempalteCoachId,
            _ => false
        };
}
