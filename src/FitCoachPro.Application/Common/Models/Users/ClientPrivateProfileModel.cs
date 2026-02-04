namespace FitCoachPro.Application.Common.Models.Users;

public record ClientPrivateProfileModel(
    Guid Id,
    string UserName,
    string Email,
    string? PhoneNumber,
    string FirstName,
    string LastName,
    DateTime CreatedAt,
    DateTime? SubscriptionExpiresAt, 
    Guid? CoachId);
