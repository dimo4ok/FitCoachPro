namespace FitCoachPro.Application.Common.Models.Users;

public record UserProfileModel(
    Guid Id, 
    string FirstName, 
    string LastName, 
    DateTime CreatedAt, 
    string? UserName = null, 
    string? PhoneNumber = null, 
    string? Email = null, 
    bool? EmailConfirmed = null);