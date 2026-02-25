namespace FitCoachPro.Application.Common.Models.Users;

public record UserProfileModel(
    Guid Id,
    string UserName,
    string FirstName,
    string LastName
    );
