using FitCoachPro.Application.Commands.Auth.SignIn;
using FitCoachPro.Application.Commands.Auth.SignUp;
using FitCoachPro.Application.Common.Models.Auth;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Identity;

namespace FitCoachPro.Tests;

public static class TestDataFactory
{
    public static SignInCommand GetSignInCommand(
        string? userName = null,
        string? password = null) =>
        new(new SignInModel
        {
            UserName = userName ?? "test-user",
            Password = password ?? "StrongPassword123!"
        });

    public static SignUpCommand GetSignUpCommand(
        string? email = null,
        string? userName = null,
        string? password = null,
        string? firstName = null,
        string? lastName = null,
        UserRole? role = null) =>
        new(new SignUpModel
        {
            Email = email ?? "user@test.com",
            UserName = userName ?? "test-user",
            Password = password ?? "StrongPassword123!",
            FirstName = firstName ?? "John",
            LastName = lastName ?? "Doe",
            Role = role ?? UserRole.Admin
        });

    public static User GetUser(
        Guid? id = null,
        string? email = null,
        string? userName = null) =>
        new()
        {
            Email = email ?? "user@test.com",
            UserName = userName ?? "test-user"
        };

    public static AuthModel GetAuthModel(
        string? token = null,
        DateTime? expires = null,
        Guid? id = null,
        string? userName = null,
        UserRole role = UserRole.Admin) =>
        new()
        {
            Token = token ?? "jwt-token",
            Expires = expires ?? DateTime.UtcNow.AddHours(1),
            Id = id ?? Guid.NewGuid(),
            UserName = userName ?? "test-user",
            Role = role
        };
}
