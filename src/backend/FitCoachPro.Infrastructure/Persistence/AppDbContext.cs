using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Domain.Entities.Workouts.Items;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using FitCoachPro.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FitCoachPro.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Admin> Admins { get; set; }

        public DbSet<Coach> Coaches { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }

        public DbSet<WorkoutItem> WorkoutItems { get; set; }

        public DbSet<TemplateWorkoutPlan> TemplateWorkoutPlans { get; set; }

        public DbSet<TemplateWorkoutItem> TemplateWorkoutItems { get; set; }

        public DbSet<Exercise> Exercises { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // Domain Users
            var clientId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var clientWithoutCoachId = Guid.Parse("33333333-3333-3333-3333-333333333334");
            var coachId = Guid.Parse("55555555-5555-5555-5555-555555555555");
            var coachWithoutClientId = Guid.Parse("55555555-5555-5555-5555-555555555556"); // new coach without clients
            var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            // Application Users
            var clientAppUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var clientWithoutCoachAppUserId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
            var coachAppUserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
            var coachWithoutClientAppUserId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"); // new app user for coach without clients
            var adminAppUserId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

            // Identity Roles (seed)
            var adminRoleId = Guid.Parse("11111111-2222-3333-4444-555555555555");
            var coachRoleId = Guid.Parse("22222222-3333-4444-5555-666666666666");
            var clientRoleId = Guid.Parse("33333333-4444-5555-6666-777777777777");

            // Seed DomainUser
            // Seed all coaches first (principals) so migrations insert them before dependent clients
            modelBuilder.Entity<Coach>().HasData(
                new Coach
                {
                    Id = coachId,
                    FirstName = "Jane",
                    LastName = "Smith",
                    CreatedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                },
                new Coach
                {
                    Id = coachWithoutClientId,
                    FirstName = "Mike",
                    LastName = "SoloCoach",
                    CreatedAt = new DateTime(2025, 9, 3, 0, 0, 0, DateTimeKind.Utc),
                }
            );

            // Seed Client that has a coach (dependent)
            modelBuilder.Entity<Client>().HasData(new Client
            {
                Id = clientId,
                FirstName = "John",
                LastName = "Doe",
                CreatedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                CoachId = coachId,
                SubscriptionExpiresAt = null
            });

            // Seed Client WITHOUT a trainer (CoachId = null)
            // (requires Client.CoachId to be nullable in the model/schema)
            modelBuilder.Entity<Client>().HasData(new Client
            {
                Id = clientWithoutCoachId,
                FirstName = "Alice",
                LastName = "NoTrainer",
                CreatedAt = new DateTime(2025, 9, 2, 0, 0, 0, DateTimeKind.Utc),
                CoachId = null,
                SubscriptionExpiresAt = null
            });

            modelBuilder.Entity<Admin>().HasData(new Admin
            {
                Id = adminId,
                FirstName = "Super",
                LastName = "Admin",
                CreatedAt = new DateTime(2025, 9, 1, 0, 0, 0, DateTimeKind.Utc),
            });

            // Seed ApplicationUser (Identity) — use stable stamps and normalized fields
            modelBuilder.Entity<AppUser>().HasData(
                new AppUser
                {
                    Id = clientAppUserId,
                    UserName = "john.doe",
                    NormalizedUserName = "JOHN.DOE",
                    Email = "john@example.com",
                    NormalizedEmail = "JOHN@EXAMPLE.COM",
                    EmailConfirmed = true,
                    DomainUserId = clientId,
                    PasswordHash = "AQAAAAEAACcQAAAAEDummyClientHash123==",
                    ConcurrencyStamp = "00000000-0000-0000-0000-000000000001",
                    SecurityStamp = "00000000-0000-0000-0000-000000000011"
                },
                new AppUser
                {
                    Id = clientWithoutCoachAppUserId,
                    UserName = "alice.notrainer",
                    NormalizedUserName = "ALICE.NOTRAINER",
                    Email = "alice@example.com",
                    NormalizedEmail = "ALICE@EXAMPLE.COM",
                    EmailConfirmed = true,
                    DomainUserId = clientWithoutCoachId,
                    PasswordHash = "AQAAAAEAACcQAAAAEDummyClientNoTrainerHash==",
                    ConcurrencyStamp = "00000000-0000-0000-0000-000000000004",
                    SecurityStamp = "00000000-0000-0000-0000-000000000014"
                },
                new AppUser
                {
                    Id = coachAppUserId,
                    UserName = "jane.smith",
                    NormalizedUserName = "JANE.SMITH",
                    Email = "jane@example.com",
                    NormalizedEmail = "JANE@EXAMPLE.COM",
                    EmailConfirmed = true,
                    DomainUserId = coachId,
                    PasswordHash = "AQAAAAEAACcQAAAAEDummyCoachHash123==",
                    ConcurrencyStamp = "00000000-0000-0000-0000-000000000002",
                    SecurityStamp = "00000000-0000-0000-0000-000000000012"
                },
                // new coach without any clients
                new AppUser
                {
                    Id = coachWithoutClientAppUserId,
                    UserName = "mike.solo",
                    NormalizedUserName = "MIKE.SOLO",
                    Email = "mike@example.com",
                    NormalizedEmail = "MIKE@EXAMPLE.COM",
                    EmailConfirmed = true,
                    DomainUserId = coachWithoutClientId,
                    PasswordHash = "AQAAAAEAACcQAAAAEDummyCoachSoloHash==",
                    ConcurrencyStamp = "00000000-0000-0000-0000-000000000005",
                    SecurityStamp = "00000000-0000-0000-0000-000000000015"
                },
                new AppUser
                {
                    Id = adminAppUserId,
                    UserName = "super.admin",
                    NormalizedUserName = "SUPER.ADMIN",
                    Email = "admin@example.com",
                    NormalizedEmail = "ADMIN@EXAMPLE.COM",
                    EmailConfirmed = true,
                    DomainUserId = adminId,
                    PasswordHash = "AQAAAAEAACcQAAAAEDummyAdminHash123==",
                    ConcurrencyStamp = "00000000-0000-0000-0000-000000000003",
                    SecurityStamp = "00000000-0000-0000-0000-000000000013"
                }
            );

            // Seed Identity Roles
            modelBuilder.Entity<IdentityRole<Guid>>().HasData(
                new IdentityRole<Guid>
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "role-concurrency-admin-1"
                },
                new IdentityRole<Guid>
                {
                    Id = coachRoleId,
                    Name = "Coach",
                    NormalizedName = "COACH",
                    ConcurrencyStamp = "role-concurrency-coach-1"
                },
                new IdentityRole<Guid>
                {
                    Id = clientRoleId,
                    Name = "Client",
                    NormalizedName = "CLIENT",
                    ConcurrencyStamp = "role-concurrency-client-1"
                }
            );

            // Seed IdentityUserRole mappings (assign users to roles)
            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
                new IdentityUserRole<Guid> { UserId = adminAppUserId, RoleId = adminRoleId },
                new IdentityUserRole<Guid> { UserId = coachAppUserId, RoleId = coachRoleId },
                new IdentityUserRole<Guid> { UserId = coachWithoutClientAppUserId, RoleId = coachRoleId },
                new IdentityUserRole<Guid> { UserId = clientAppUserId, RoleId = clientRoleId },
                new IdentityUserRole<Guid> { UserId = clientWithoutCoachAppUserId, RoleId = clientRoleId }
            );

            // Seed TemplateWorkoutPlans
            var templatePlanId1 = Guid.Parse("31000000-0000-0000-0000-000000000001");
            var templatePlanId2 = Guid.Parse("31000000-0000-0000-0000-000000000002");

            modelBuilder.Entity<TemplateWorkoutPlan>().HasData(
                new TemplateWorkoutPlan
                {
                    Id = templatePlanId1,
                    CoachId = coachId,
                    CreatedAt = new DateTime(2025, 9, 5, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = null
                },
                new TemplateWorkoutPlan
                {
                    Id = templatePlanId2,
                    CoachId = coachId,
                    CreatedAt = new DateTime(2025, 9, 6, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = null
                }
            );

            // Seed TemplateWorkoutItems
            modelBuilder.Entity<TemplateWorkoutItem>().HasData(
                new TemplateWorkoutItem
                {
                    Id = Guid.Parse("32000000-0000-0000-0000-000000000001"),
                    TemplateWorkoutPlanId = templatePlanId1,
                    ExerciseId = Guid.Parse("21000000-0000-0000-0000-000000000001"),
                    Description = "3 sets of 12 push-ups"
                },
                new TemplateWorkoutItem
                {
                    Id = Guid.Parse("32000000-0000-0000-0000-000000000002"),
                    TemplateWorkoutPlanId = templatePlanId2,
                    ExerciseId = Guid.Parse("21000000-0000-0000-0000-000000000002"),
                    Description = "4 sets of 10 squats"
                }
            );

            // Seed Exercises
            modelBuilder.Entity<Exercise>().HasData(
                new Exercise
                {
                    Id = Guid.Parse("21000000-0000-0000-0000-000000000001"),
                    ExerciseName = "Push-up",
                    GifUrl = "https://example.com/gifs/pushup.gif"
                },
                new Exercise
                {
                    Id = Guid.Parse("21000000-0000-0000-0000-000000000002"),
                    ExerciseName = "Squat",
                    GifUrl = "https://example.com/gifs/squat.gif"
                }
            );

            // Seed WorkoutPlans
            var workoutPlanId = Guid.Parse("41000000-0000-0000-0000-000000000001");
            modelBuilder.Entity<WorkoutPlan>().HasData(
                new WorkoutPlan
                {
                    Id = workoutPlanId,
                    ClientId = clientId,
                    DateOfDoing = new DateTime(2025, 9, 10, 8, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed WorkoutItems
            modelBuilder.Entity<WorkoutItem>().HasData(
                new WorkoutItem
                {
                    Id = Guid.Parse("51000000-0000-0000-0000-000000000001"),
                    WorkoutPlanId = workoutPlanId,
                    ExerciseId = Guid.Parse("21000000-0000-0000-0000-000000000001"),
                    Description = "Morning push-up routine - 3x12"
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
