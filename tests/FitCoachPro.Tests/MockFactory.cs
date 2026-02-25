using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace FitCoachPro.Tests;

public static class MockFactory
{
    public static UserManager<TUser> GetMockUserManager<TUser>()
       where TUser : class
    {
        var store = Substitute.For<IUserStore<TUser>>();
        var passwordHasher = Substitute.For<IPasswordHasher<TUser>>();
        IList<IUserValidator<TUser>> userValidators = new List<IUserValidator<TUser>>
        {
            new UserValidator<TUser>()
        };

        IList<IPasswordValidator<TUser>> passwordValidators = new List<IPasswordValidator<TUser>>
        {
            new PasswordValidator<TUser>()
        };

        userValidators.Add(new UserValidator<TUser>());
        passwordValidators.Add(new PasswordValidator<TUser>());

        var userManager = Substitute.For<UserManager<TUser>>(store, null, passwordHasher, userValidators, passwordValidators, null, null, null, null);

        return userManager;
    }

    public static RoleManager<IdentityRole<Guid>> GetMockRoleManager()
    {
        var store = Substitute.For<IRoleStore<IdentityRole<Guid>>>();
        var roleValidators = new List<IRoleValidator<IdentityRole<Guid>>>();
        var keyNormalizer = Substitute.For<ILookupNormalizer>();
        var errors = Substitute.For<IdentityErrorDescriber>();
        var logger = Substitute.For<ILogger<RoleManager<IdentityRole<Guid>>>>();

        var roleManager = Substitute.For<RoleManager<IdentityRole<Guid>>>(
            store,
            roleValidators,
            keyNormalizer,
            errors,
            logger
        );

        return roleManager;
    }
}
