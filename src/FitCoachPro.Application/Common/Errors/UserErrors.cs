using FitCoachPro.Application.Common.Response;

namespace FitCoachPro.Application.Common.Errors;

public class UserErrors
{
    public static Error EmailAlreadyExists => new("User.EmailAlreadyExists", "Email already exists.");
    public static Error PhoneAlreadyExists => new("User.PhoneAlreadyExists", "Phone already exists.");
    public static Error InvalidCredentials => new("User.InvalidCredentials", "The username or password is incorrect.");
    public static Error NotFound => new("User.NotFound", "User not found.");
    public static Error RoleNotFound => new("User.RoleNotFound", "The user role is not found.");
    public static Error InvalidRole => new("User.InvalidRole", "The specified role is invalid.");

    public static Error RelationshipNotFound(string fromEntity, string toEntity) =>
      new(
          $"{fromEntity}_{toEntity}_RelationshipNotFound",
          $"{fromEntity} does not have a related {toEntity}.");

    public static Error ActiveSubscriptionPreventsUnassign =>
        new(
            "User.ActiveSubscriptionPreventsUnassign",
            "You cannot unassign this relationship while the client’s subscription is still active.");

    public static Error CoachHasActiveClientsDeleteAccount =>
         new(
             "User.CoachHasActiveClientsDeleteAccount",
             "You cannot delete your account because you still have assigned clients.");

    public static Error ClientHasAssignedCoachDeleteAccount =>
        new(
            "User.ClientHasAssignedCoachDeleteAccount",
            "You cannot delete your account while you are assigned to a coach.");

    public static Error PasswordsDoNotMatch =>
    new(
        "User.PasswordsDoNotMatch",
        "The new password and confirmation password do not match.");

}
