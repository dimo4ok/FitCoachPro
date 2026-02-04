using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Domain.Entities.Users;

namespace FitCoachPro.Application.Common.Extensions;

public static class UserExtensions
{
    public static UserProfileModel ToModel(this UserProfile userProfile) =>
        new(
            userProfile.Id,
            userProfile.User.UserName!,
            userProfile.FirstName,
            userProfile.LastName
            );

    public static CoachPublicProfileModel ToPublicModel(this Coach coach) =>
        new(
            coach.Id,
            coach.User.UserName!,
            coach.User.Email!,
            coach.User.PhoneNumber,
            coach.FirstName,
            coach.LastName,
            coach.CreatedAt,
            coach.AcceptanceStatus);

    public static ClientPublicProfileModel ToPublicModel(this Client client) =>
        new(
            client.Id,
            client.User.UserName!,
            client.User.Email!,
            client.User.PhoneNumber,
            client.FirstName,
            client.LastName,
            client.CreatedAt,
            client.CoachId != null);

    public static AdminPublicProfileModel ToPublicModel(this Admin admin) =>
       new(
           admin.Id,
           admin.User.UserName!,
           admin.User.Email!,
           admin.User.PhoneNumber,
           admin.FirstName,
           admin.LastName,
           admin.CreatedAt);

    public static ClientPrivateProfileModel ToPrivateModel(this Client client) =>
        new(
            client.Id,
            client.User.UserName!,
            client.User.Email!,
            client.User.PhoneNumber,
            client.FirstName,
            client.LastName,
            client.CreatedAt,
            client.SubscriptionExpiresAt,
            client.CoachId);

    public static CoachPrivateProfileModel ToPrivateModel(this Coach coach) =>
        new(
            coach.Id,
            coach.User.UserName!,
            coach.User.Email!,
            coach.User.PhoneNumber,
            coach.FirstName,
            coach.LastName,
            coach.CreatedAt,
            coach.AcceptanceStatus);

    public static AdminPrivateProfileModel ToPrivateModel(this Admin admin) =>
        new(
            admin.Id,
            admin.User.UserName!,
            admin.User.Email!,
            admin.User.PhoneNumber,
            admin.FirstName,
            admin.LastName,
            admin.CreatedAt);
}
