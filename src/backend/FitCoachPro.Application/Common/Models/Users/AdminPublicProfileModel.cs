namespace FitCoachPro.Application.Common.Models.Users;

public record AdminPublicProfileModel(
    Guid Id,
    string UserName,
    string Email,
    string? PhoneNumber,
    string FirstName,
    string LastName,
    DateTime CreatedAt);