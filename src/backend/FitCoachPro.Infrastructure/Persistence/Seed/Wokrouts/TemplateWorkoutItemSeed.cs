using FitCoachPro.Domain.Entities.Workouts.Items;

namespace FitCoachPro.Infrastructure.Persistence.Seed.Wokrouts;

public static class TemplateWorkoutItemSeed
{
    public static void Seed(AppDbContext context)
    {
        if (!context.TemplateWorkoutItems.Any())
        {
            var items = new List<TemplateWorkoutItem>
            {
                // --- Template Plan 1
                new TemplateWorkoutItem
                {
                    Id = Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                    Description = "3 sets of 10 reps",
                    TemplateWorkoutPlanId = Guid.Parse("bbbbbbb1-bbbb-bbbb-bbbb-bbbbbbbbbbb1"),
                    ExerciseId = Guid.Parse("11111111-1111-1111-1111-111111111111")
                },
                new TemplateWorkoutItem
                {
                    Id = Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                    Description = "3 sets of 12 reps",
                    TemplateWorkoutPlanId = Guid.Parse("bbbbbbb1-bbbb-bbbb-bbbb-bbbbbbbbbbb1"),
                    ExerciseId = Guid.Parse("11111111-1111-1111-1111-111111111111")
                },

                // --- Template Plan 2
                new TemplateWorkoutItem
                {
                    Id = Guid.Parse("a3333333-3333-3333-3333-333333333333"),
                    Description = "4 sets of 6 reps",
                    TemplateWorkoutPlanId = Guid.Parse("bbbbbbb2-bbbb-bbbb-bbbb-bbbbbbbbbbb2"),
                    ExerciseId = Guid.Parse("11111111-1111-1111-1111-111111111111")
                },
                new TemplateWorkoutItem
                {
                    Id = Guid.Parse("a4444444-4444-4444-4444-444444444444"),
                    Description = "3 sets of 8 reps",
                    TemplateWorkoutPlanId = Guid.Parse("bbbbbbb2-bbbb-bbbb-bbbb-bbbbbbbbbbb2"),
                    ExerciseId = Guid.Parse("11111111-1111-1111-1111-111111111111")
                }
            };

            context.TemplateWorkoutItems.AddRange(items);
            context.SaveChanges();
        }
    }
}
