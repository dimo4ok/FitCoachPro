using FitCoachPro.Domain.Entities.Enums;

namespace FitCoachPro.API.Common;

public static class AuthorizationPolicies
{
    public static readonly string Admin = UserRole.Admin.ToString();
    public static readonly string Coach = UserRole.Coach.ToString();
    public static readonly string Client = UserRole.Client.ToString();
}
