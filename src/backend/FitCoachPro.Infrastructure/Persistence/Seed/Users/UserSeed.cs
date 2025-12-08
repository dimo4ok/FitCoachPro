using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;

namespace FitCoachPro.Infrastructure.Persistence.Seed.Users;

public static class UserSeed
{
    public static void Seed(AppDbContext context)
    {
        // --- Admin ---
        if (!context.Admins.Any())
        {
            context.Admins.Add(new Admin
            {
                Id = Guid.Parse("00000001-0000-0000-0000-000000000001"),
                FirstName = "Admin",
                LastName = "User",
                CreatedAt = DateTime.UtcNow,
                UserId = Guid.Parse("00000010-0000-0000-0000-000000000010") 
            });
        }

        // --- Coaches ---
        var coach1 = context.Coaches.FirstOrDefault(c => c.Id == Guid.Parse("00000002-0000-0000-0000-000000000002"));
        var coach2 = context.Coaches.FirstOrDefault(c => c.Id == Guid.Parse("00000003-0000-0000-0000-000000000003"));

        if (coach1 == null)
        {
            coach1 = new Coach
            {
                Id = Guid.Parse("00000002-0000-0000-0000-000000000002"),
                FirstName = "John",
                LastName = "Doe",
                CreatedAt = DateTime.UtcNow,
                AcceptanceStatus = ClientAcceptanceStatus.Accepting,
                UserId = Guid.Parse("00000011-0000-0000-0000-000000000011"),
                Clients = new List<Client>()
            };
            context.Coaches.Add(coach1);
        }

        if (coach2 == null)
        {
            coach2 = new Coach
            {
                Id = Guid.Parse("00000003-0000-0000-0000-000000000003"),
                FirstName = "Jane",
                LastName = "Smith",
                CreatedAt = DateTime.UtcNow,
                AcceptanceStatus = ClientAcceptanceStatus.Accepting,
                UserId = Guid.Parse("00000012-0000-0000-0000-000000000012"),
                Clients = new List<Client>()
            };
            context.Coaches.Add(coach2);
        }

        // --- Clients ---
        if (!context.Clients.Any())
        {
            var client1 = new Client
            {
                Id = Guid.Parse("00000020-0000-0000-0000-000000000020"),
                FirstName = "Client",
                LastName = "One",
                CreatedAt = DateTime.UtcNow,
                UserId = Guid.Parse("00000020-0000-0000-0000-000000000020"),
                CoachId = coach1.Id
            };

            var client2 = new Client
            {
                Id = Guid.Parse("00000021-0000-0000-0000-000000000021"),
                FirstName = "Client",
                LastName = "Two",
                CreatedAt = DateTime.UtcNow,
                UserId = Guid.Parse("00000021-0000-0000-0000-000000000021"),
                CoachId = coach2.Id
            };

            var client3 = new Client
            {
                Id = Guid.Parse("00000022-0000-0000-0000-000000000022"),
                FirstName = "Client",
                LastName = "Three",
                CreatedAt = DateTime.UtcNow,
                UserId = Guid.Parse("00000022-0000-0000-0000-000000000022"),
                CoachId = coach2.Id
            };

            context.Clients.AddRange(client1, client2, client3);

            coach1.Clients.Add(client1);
            coach2.Clients.Add(client2);
            coach2.Clients.Add(client3);
        }

        context.SaveChanges();
    }
}