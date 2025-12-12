using FitCoachPro.Application.Common.Models;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Domain.Entities;
using FitCoachPro.Domain.Entities.Enums;
namespace FitCoachPro.Application.Services.Access;

public class ClientCoachRequestAccessService : IClientCoachRequestAccessService
{
    public bool HasAccesToRequest(ClientCoachRequest request, UserContext userContext) =>
    userContext.Role switch
    {
        UserRole.Admin => true,
        UserRole.Coach => request.CoachId == userContext.UserId,
        UserRole.Client => request.ClientId == userContext.UserId,
        _ => false
    };
}
