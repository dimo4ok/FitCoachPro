using FitCoachPro.Domain.Entities;
using FitCoachPro.Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }

        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }

        public DbSet<WorkoutItem> WorkoutItems { get; set; }

        public DbSet<Exercise> Exercises { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // --- Seed Users ---
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    FirstName = "John",
                    LastName = "Doe",
                    TelephoneNumber = "+123456789",
                    PasswordHash = "hash1",
                    Role = UserRole.User,
                    DateOfRegistration = new DateTime(2025, 7, 11)
                },
                new User
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    FirstName = "Anna",
                    LastName = "Smith",
                    TelephoneNumber = "+987654321",
                    PasswordHash = "hash2",
                    Role = UserRole.User,
                    DateOfRegistration = new DateTime(2025, 7, 10)
                },
                new User
                {
                    Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    FirstName = "Mike",
                    LastName = "Johnson",
                    TelephoneNumber = "+192837465",
                    PasswordHash = "hash3",
                    Role = UserRole.User,
                    DateOfRegistration = new DateTime(2025, 7, 9)
                }
            );

            // --- Seed WorkoutPlans ---
            modelBuilder.Entity<WorkoutPlan>().HasData(
                new
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    DateOfDoing = new DateTime(2025, 7, 15)
                },
                new
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    DateOfDoing = new DateTime(2025, 7, 16)
                },
                new
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    UserId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    DateOfDoing = new DateTime(2025, 7, 17)
                }
            );

            // --- Seed Exercises ---
            modelBuilder.Entity<Exercise>().HasData(
                new Exercise
                {
                    Id = Guid.Parse("aaaaaaaa-1111-1111-1111-111111111111"),
                    ExerciseName = "Push-ups",
                    GifUrl = "https://example.com/gifs/pushups.gif"
                },
                new Exercise
                {
                    Id = Guid.Parse("bbbbbbbb-2222-2222-2222-222222222222"),
                    ExerciseName = "Squats",
                    GifUrl = "https://example.com/gifs/squats.gif"
                },
                new Exercise
                {
                    Id = Guid.Parse("cccccccc-3333-3333-3333-333333333333"),
                    ExerciseName = "Plank",
                    GifUrl = "https://example.com/gifs/plank.gif"
                },
                new Exercise
                {
                    Id = Guid.Parse("dddddddd-4444-4444-4444-444444444444"),
                    ExerciseName = "Lunges",
                    GifUrl = "https://example.com/gifs/lunges.gif"
                },
                new Exercise
                {
                    Id = Guid.Parse("eeeeeeee-5555-5555-5555-555555555555"),
                    ExerciseName = "Pull-ups",
                    GifUrl = "https://example.com/gifs/pullups.gif"
                },
                new Exercise
                {
                    Id = Guid.Parse("ffffffff-6666-6666-6666-666666666666"),
                    ExerciseName = "Deadlifts",
                    GifUrl = "https://example.com/gifs/deadlifts.gif"
                }
            );

            // --- Seed WorkoutItems ---
            modelBuilder.Entity<WorkoutItem>().HasData(
                // John
                new
                {
                    Id = Guid.Parse("aaaa1111-0000-0000-0000-000000000001"),
                    WorkoutPlanId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    ExerciseId = Guid.Parse("aaaaaaaa-1111-1111-1111-111111111111"),
                    Description = "3 sets of 15 reps"
                },
                new
                {
                    Id = Guid.Parse("aaaa1111-0000-0000-0000-000000000002"),
                    WorkoutPlanId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    ExerciseId = Guid.Parse("bbbbbbbb-2222-2222-2222-222222222222"),
                    Description = "3 sets of 20 reps"
                },

                // Anna
                new
                {
                    Id = Guid.Parse("bbbb2222-0000-0000-0000-000000000001"),
                    WorkoutPlanId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    ExerciseId = Guid.Parse("cccccccc-3333-3333-3333-333333333333"),
                    Description = "Hold for 60 seconds"
                },
                new
                {
                    Id = Guid.Parse("bbbb2222-0000-0000-0000-000000000002"),
                    WorkoutPlanId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    ExerciseId = Guid.Parse("dddddddd-4444-4444-4444-444444444444"),
                    Description = "3 sets of 10 reps per leg"
                },

                // Mike
                new
                {
                    Id = Guid.Parse("cccc3333-0000-0000-0000-000000000001"),
                    WorkoutPlanId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    ExerciseId = Guid.Parse("eeeeeeee-5555-5555-5555-555555555555"),
                    Description = "3 sets to failure"
                },
                new
                {
                    Id = Guid.Parse("cccc3333-0000-0000-0000-000000000002"),
                    WorkoutPlanId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    ExerciseId = Guid.Parse("ffffffff-6666-6666-6666-666666666666"),
                    Description = "3 sets of 8 reps"
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
