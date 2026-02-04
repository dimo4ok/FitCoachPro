namespace FitCoachPro.Application.Common.Models.Users;

public record UpdatePasswordModel(string Password, string NewPassword, string ConfirmPassword);
