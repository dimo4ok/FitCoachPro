using FitCoachPro.Domain.Entities.Workouts;

namespace FitCoachPro.Infrastructure.Persistence.Seed.Wokrouts;

public static class ExerciseSeed
{
    public static void Seed(AppDbContext context)
    {
        if (!context.Exercises.Any())
        {
            var exercises = new List<Exercise>
            {
                new() {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    ExerciseName = "Bench Press",
                    GifUrl = "https://example.com/gifs/bench-press.gif"
                },
                new() {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    ExerciseName = "Squat",
                    GifUrl = "https://example.com/gifs/squat.gif"
                },
                new() {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    ExerciseName = "Deadlift",
                    GifUrl = "https://example.com/gifs/deadlift.gif"
                },
                new() {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    ExerciseName = "Pull Up",
                    GifUrl = "https://example.com/gifs/pull-up.gif"
                },
                new() {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    ExerciseName = "Overhead Press",
                    GifUrl = "https://example.com/gifs/overhead-press.gif"
                }
            };

            context.Exercises.AddRange(exercises);
            context.SaveChanges();
        }
    }
}
