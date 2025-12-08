using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Domain.Entities.Users;

namespace FitCoachPro.Application.Common.Extensions;

public static class UserProfleExtensions
{
    public static UserProfileModel ToModel(this UserProfile userProfile) =>
        new(
            userProfile.Id,
            userProfile.FirstName,
            userProfile.LastName,
            userProfile.CreatedAt);
}
