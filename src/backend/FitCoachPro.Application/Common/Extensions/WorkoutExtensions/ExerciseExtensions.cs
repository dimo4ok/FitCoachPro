using FitCoachPro.Application.Common.Models.Workouts.Exercise;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Domain.Entities.Workouts.Plans;

namespace FitCoachPro.Application.Common.Extensions.WorkoutExtensions;

public static class ExerciseExtensions
{
    public static Exercise ToEntity(this CreateExerciseModel model) =>
        new()
        {
            ExerciseName = model.ExerciseName,
            GifUrl = model.GifUrl,
        };

    public static ExerciseDetailModel ToModel(this Exercise exercise) =>
       new(
           exercise.Id,
           exercise.ExerciseName,
           exercise.GifUrl,
           exercise.RowVersion);

    public static ExerciseModel ToNestedModel(this Exercise exercise) =>
        new(
            exercise.Id,
            exercise.ExerciseName,
            exercise.GifUrl);

    public static IReadOnlyList<ExerciseDetailModel> ToModel(this IReadOnlyList<Exercise> exercise) =>
        exercise
            .Select(x => x.ToModel())
            .ToList()
            .AsReadOnly();
}
