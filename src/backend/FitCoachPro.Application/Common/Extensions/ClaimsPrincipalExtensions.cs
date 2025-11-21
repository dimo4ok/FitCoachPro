using FitCoachPro.Application.Common.Models;
using FitCoachPro.Domain.Entities.Enums;
using System.Security.Claims;

namespace FitCoachPro.Application.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static UserContext ToUserContext(this ClaimsPrincipal user)
    {
        return new UserContext(
            Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!),
            Enum.Parse<UserRole>(user.FindFirstValue(ClaimTypes.Role)!)
            );
    }
}
