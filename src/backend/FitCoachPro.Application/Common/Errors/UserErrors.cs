using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Common.Errors;

public class UserErrors
{
    public static Error EmailAlreadyExists => new("User.EmailAlreadyExists", "Email already exists.");
    public static Error NotFound => new("User.NotFound", "User not found.");
    public static Error WrongPassword => new("User.WrongPassword", "The password provided is incorrect.");
    public static Error RoleNotFound => new("User.RoleNotFound", "The user role is not found.");
}
