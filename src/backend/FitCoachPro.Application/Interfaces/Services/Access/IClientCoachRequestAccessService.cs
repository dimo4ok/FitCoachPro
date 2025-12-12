using FitCoachPro.Application.Common.Models;
using FitCoachPro.Domain.Entities;

namespace FitCoachPro.Application.Interfaces.Services.Access;

public interface IClientCoachRequestAccessService
{
    bool HasAccesToRequest(ClientCoachRequest request, UserContext userContext);
}