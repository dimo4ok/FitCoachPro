using FitCoachPro.Domain.Entities.Workouts.Items;

namespace FitCoachPro.Infrastructure.Persistence.Seed;

public static class WorkoutItemSeed
{
    public static void Seed(AppDbContext context)
    {
        if (!context.WorkoutItems.Any())
        {
            var items = new List<WorkoutItem>
            {
                // --- Client 1
                new WorkoutItem
                {
                    Id = Guid.Parse("bbbbbbb1-bbbb-bbbb-bbbb-bbbbbbbbbbb1"),
                    Description = "3 sets of 10 reps",
                    WorkoutPlanId = Guid.Parse("aaaaaaa1-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
                    ExerciseId = Guid.Parse("11111111-1111-1111-1111-111111111111") 
                },

                // --- Client 1
                new WorkoutItem
                {
                    Id = Guid.Parse("bbbbbbb2-bbbb-bbbb-bbbb-bbbbbbbbbbb2"),
                    Description = "3 sets of 12 reps",
                    WorkoutPlanId = Guid.Parse("aaaaaaa2-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                    ExerciseId = Guid.Parse("22222222-2222-2222-2222-222222222222")
                },
                new WorkoutItem
                {
                    Id = Guid.Parse("bbbbbbb3-bbbb-bbbb-bbbb-bbbbbbbbbbb3"),
                    Description = "3 sets of 8 reps",
                    WorkoutPlanId = Guid.Parse("aaaaaaa2-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                    ExerciseId = Guid.Parse("33333333-3333-3333-3333-333333333333") 
                },

                // --- Client 2
                new WorkoutItem
                {
                    Id = Guid.Parse("bbbbbbb4-bbbb-bbbb-bbbb-bbbbbbbbbbb4"),
                    Description = "4 sets of 5 reps",
                    WorkoutPlanId = Guid.Parse("aaaaaaa3-aaaa-aaaa-aaaa-aaaaaaaaaaa3"),
                    ExerciseId = Guid.Parse("44444444-4444-4444-4444-444444444444") 
                },
                new WorkoutItem
                {
                    Id = Guid.Parse("bbbbbbb5-bbbb-bbbb-bbbb-bbbbbbbbbbb5"),
                    Description = "4 sets of 6 reps",
                    WorkoutPlanId = Guid.Parse("aaaaaaa3-aaaa-aaaa-aaaa-aaaaaaaaaaa3"),
                    ExerciseId = Guid.Parse("55555555-5555-5555-5555-555555555555") 
                },

                // --- Client 3
                new WorkoutItem
                {
                    Id = Guid.Parse("bbbbbbb6-bbbb-bbbb-bbbb-bbbbbbbbbbb6"),
                    Description = "3 sets of 8 reps",
                    WorkoutPlanId = Guid.Parse("aaaaaaa4-aaaa-aaaa-aaaa-aaaaaaaaaaa4"),
                    ExerciseId = Guid.Parse("22222222-2222-2222-2222-222222222222") 
                },
                new WorkoutItem
                {
                    Id = Guid.Parse("bbbbbbb7-bbbb-bbbb-bbbb-bbbbbbbbbbb7"),
                    Description = "3 sets of 10 reps",
                    WorkoutPlanId = Guid.Parse("aaaaaaa4-aaaa-aaaa-aaaa-aaaaaaaaaaa4"),
                    ExerciseId = Guid.Parse("11111111-1111-1111-1111-111111111111") 
                }
            };

            context.WorkoutItems.AddRange(items);
            context.SaveChanges();
        }
    }
}
