using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Common.Errors;

public class UserErrors
{
    public static Error EmailAlreadyExists => new("User.EmailAlreadyExists", "Email already exists.");
    public static Error NotFound => new("User.NotFound", "User not found.");
    public static Error WrongPassword => new("User.WrongPassword", "The password provided is incorrect.");
    public static Error RoleNotFound => new("User.RoleNotFound", "The user role is not found.");
    public static Error InvalidRole => new("User.InvalidRole", "The specified role is invalid.");

    public static Error FirstNameRequired => new("User.FirstNameRequired", "First name must be provided.");
    public static Error LastNameRequired => new("User.LastNameRequired", "Last name must be provided.");
    public static Error EmailRequired => new("User.EmailRequired", "Email must be provided.");
    public static Error EmailInvalid => new("User.EmailInvalid", "Email format is invalid.");
    public static Error UserNameRequired => new("User.UserNameRequired", "UserName must be provided.");
    public static Error UserNameInvalidLength => new("User.UserNameInvalidLength", "UserName must be between 3 and 20 characters.");
    public static Error PasswordRequired => new("User.PasswordRequired", "Password must be provided.");
}
