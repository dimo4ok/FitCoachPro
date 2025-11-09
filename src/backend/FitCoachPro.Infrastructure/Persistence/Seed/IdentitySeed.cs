using FitCoachPro.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace FitCoachPro.Infrastructure.Persistence.Seed;

public static class IdentitySeed
{
    public static void Seed(AppDbContext context)
    {
        var passwordHasher = new PasswordHasher<User>();

        // --- Roles ---
        if (!context.Roles.Any())
        {
            var roles = new List<IdentityRole<Guid>>
            {
                new() { Id = Guid.Parse("00000001-0000-0000-0000-000000000001"), Name = "Admin", NormalizedName = "ADMIN" },
                new() { Id = Guid.Parse("00000002-0000-0000-0000-000000000002"), Name = "Coach", NormalizedName = "COACH" },
                new() { Id = Guid.Parse("00000003-0000-0000-0000-000000000003"), Name = "Client", NormalizedName = "CLIENT" }
            };
            context.Roles.AddRange(roles);
        }

        // --- Users ---
        if (!context.Users.Any())
        {
            var users = new List<User>
            {
                new()
                {
                    Id = Guid.Parse("00000010-0000-0000-0000-000000000010"),
                    UserName = "admin@test.com",
                    NormalizedUserName = "ADMIN@TEST.COM",
                    Email = "admin@test.com",
                    NormalizedEmail = "ADMIN@TEST.COM",
                    PasswordHash = passwordHasher.HashPassword(null!, "Admin123!"),
                    EmailConfirmed = true
                },
                new()
                {
                    Id = Guid.Parse("00000011-0000-0000-0000-000000000011"),
                    UserName = "coach1@test.com",
                    NormalizedUserName = "COACH1@TEST.COM",
                    Email = "coach1@test.com",
                    NormalizedEmail = "COACH1@TEST.COM",
                    PasswordHash = passwordHasher.HashPassword(null!, "Coach123!"),
                    EmailConfirmed = true
                },
                new()
                {
                    Id = Guid.Parse("00000012-0000-0000-0000-000000000012"),
                    UserName = "coach2@test.com",
                    NormalizedUserName = "COACH2@TEST.COM",
                    Email = "coach2@test.com",
                    NormalizedEmail = "COACH2@TEST.COM",
                    PasswordHash = passwordHasher.HashPassword(null!, "Coach123!"),
                    EmailConfirmed = true
                },
                new()
                {
                    Id = Guid.Parse("00000020-0000-0000-0000-000000000020"),
                    UserName = "client1@test.com",
                    NormalizedUserName = "CLIENT1@TEST.COM",
                    Email = "client1@test.com",
                    NormalizedEmail = "CLIENT1@TEST.COM",
                    PasswordHash = passwordHasher.HashPassword(null!, "Client123!"),
                    EmailConfirmed = true
                },
                new()
                {
                    Id = Guid.Parse("00000021-0000-0000-0000-000000000021"),
                    UserName = "client2@test.com",
                    NormalizedUserName = "CLIENT2@TEST.COM",
                    Email = "client2@test.com",
                    NormalizedEmail = "CLIENT2@TEST.COM",
                    PasswordHash = passwordHasher.HashPassword(null!, "Client123!"),
                    EmailConfirmed = true
                },
                new()
                {
                    Id = Guid.Parse("00000022-0000-0000-0000-000000000022"),
                    UserName = "client3@test.com",
                    NormalizedUserName = "CLIENT3@TEST.COM",
                    Email = "client3@test.com",
                    NormalizedEmail = "CLIENT3@TEST.COM",
                    PasswordHash = passwordHasher.HashPassword(null!, "Client123!"),
                    EmailConfirmed = true
                }
            };

            context.Users.AddRange(users);
        }

        // --- UserRoles ---
        if (!context.UserRoles.Any())
        {
            var userRoles = new List<IdentityUserRole<Guid>>
            {
                new() { UserId = Guid.Parse("00000010-0000-0000-0000-000000000010"), RoleId = Guid.Parse("00000001-0000-0000-0000-000000000001") }, // Admin
                new() { UserId = Guid.Parse("00000011-0000-0000-0000-000000000011"), RoleId = Guid.Parse("00000002-0000-0000-0000-000000000002") }, // Coach1
                new() { UserId = Guid.Parse("00000012-0000-0000-0000-000000000012"), RoleId = Guid.Parse("00000002-0000-0000-0000-000000000002") }, // Coach2
                new() { UserId = Guid.Parse("00000020-0000-0000-0000-000000000020"), RoleId = Guid.Parse("00000003-0000-0000-0000-000000000003") }, // Client1
                new() { UserId = Guid.Parse("00000021-0000-0000-0000-000000000021"), RoleId = Guid.Parse("00000003-0000-0000-0000-000000000003") }, // Client2
                new() { UserId = Guid.Parse("00000022-0000-0000-0000-000000000022"), RoleId = Guid.Parse("00000003-0000-0000-0000-000000000003") }  // Client3
            };

            context.UserRoles.AddRange(userRoles);
        }

        context.SaveChanges();
    }
}
