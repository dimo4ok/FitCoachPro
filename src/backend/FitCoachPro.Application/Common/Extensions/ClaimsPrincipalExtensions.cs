using FitCoachPro.Application.Common.Models;
using FitCoachPro.Domain.Entities.Enums;
using System.Security.Claims;

namespace FitCoachPro.Application.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static CurrentUserModel ToCurrentUser(this ClaimsPrincipal user)
    {
        return new CurrentUserModel(
            Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!),
            Enum.Parse<UserRole>(user.FindFirstValue(ClaimTypes.Role)!)
            );
    }
}
