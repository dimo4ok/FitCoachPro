using FitCoachPro.Domain.Entities.Workouts.Plans;

namespace FitCoachPro.Infrastructure.Persistence.Seed;

public class TemplateWorkoutPlanSeed
{
    public static void Seed(AppDbContext context)
    {
        if (!context.TemplateWorkoutPlans.Any())
        {
            var templates = new List<TemplateWorkoutPlan>
            {
                // --- Coach 1 ---
                new TemplateWorkoutPlan
                {
                    Id = Guid.Parse("bbbbbbb1-bbbb-bbbb-bbbb-bbbbbbbbbbb1"),
                    TemplateName = "Full Body Beginner Plan",
                    CoachId = Guid.Parse("00000002-0000-0000-0000-000000000002"),
                    CreatedAt = new DateTime(2025, 11, 12),
                },

                // --- Coach 2 ---
                new TemplateWorkoutPlan
                {
                    Id = Guid.Parse("bbbbbbb2-bbbb-bbbb-bbbb-bbbbbbbbbbb2"),
                    TemplateName = "Upper Body Strength Plan",
                    CoachId = Guid.Parse("00000003-0000-0000-0000-000000000003"),
                    CreatedAt = new DateTime(2025, 11, 12),
                }
            };

            context.TemplateWorkoutPlans.AddRange(templates);
            context.SaveChanges();
        }
    }
}
