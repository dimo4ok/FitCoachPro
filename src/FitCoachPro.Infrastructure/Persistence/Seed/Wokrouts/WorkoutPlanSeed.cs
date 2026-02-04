using FitCoachPro.Domain.Entities.Workouts.Plans;

namespace FitCoachPro.Infrastructure.Persistence.Seed.Wokrouts;

public static class WorkoutPlanSeed
{
    public static void Seed(AppDbContext context)
    {
        if (!context.WorkoutPlans.Any())
        {
            var plans = new List<WorkoutPlan>
            {
                // --- Client 1 ---
                new() {
                    Id = Guid.Parse("aaaaaaa1-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
                    ClientId = Guid.Parse("00000020-0000-0000-0000-000000000020"),
                    WorkoutDate = new DateTime(2025, 11, 10)
                },
                new() {
                    Id = Guid.Parse("aaaaaaa2-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                    ClientId = Guid.Parse("00000020-0000-0000-0000-000000000020"),
                    WorkoutDate = new DateTime(2025, 11, 12)
                },

                // --- Client 2 ---
                new() {
                    Id = Guid.Parse("aaaaaaa3-aaaa-aaaa-aaaa-aaaaaaaaaaa3"),
                    ClientId = Guid.Parse("00000021-0000-0000-0000-000000000021"),
                    WorkoutDate = new DateTime(2025, 11, 11)
                },

                // --- Client 3 ---
                new() {
                    Id = Guid.Parse("aaaaaaa4-aaaa-aaaa-aaaa-aaaaaaaaaaa4"),
                    ClientId = Guid.Parse("00000022-0000-0000-0000-000000000022"),
                    WorkoutDate = new DateTime(2025, 11, 10)
                }
            };

            context.WorkoutPlans.AddRange(plans);
            context.SaveChanges();
        }
    }
}
