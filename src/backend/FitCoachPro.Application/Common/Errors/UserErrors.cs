using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Common.Errors;

public class UserErrors
{
    public static Error EmailAlreadyExists => new("User.EmailAlreadyExists", "Email already exists.");
    public static Error InvalidCredentials => new("User.InvalidCredentials", "The username or password is incorrect.");
    public static Error NotFound => new("User.NotFound", "User not found.");
    public static Error RoleNotFound => new("User.RoleNotFound", "The user role is not found.");
    public static Error InvalidRole => new("User.InvalidRole", "The specified role is invalid.");
}
