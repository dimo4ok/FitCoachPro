using FitCoachPro.Application.Common.Models.Workouts.Exercise;
using FitCoachPro.Domain.Entities.Workouts;

namespace FitCoachPro.Application.Common.Extensions.WorkoutExtensions;

public static class ExerciseExtensions
{
    public static Exercise ToEntity(this CreateExerciseModel model) =>
        new()
        {
            ExerciseName = model.ExerciseName,
            GifUrl = model.GifUrl,
        };

    public static ExerciseModel ToModel(this Exercise exercise) =>
        new(
            exercise.Id,
            exercise.ExerciseName,
            exercise.GifUrl);
}
