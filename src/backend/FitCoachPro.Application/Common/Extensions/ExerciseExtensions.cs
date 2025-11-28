using FitCoachPro.Application.Common.Models.Exercise;
using FitCoachPro.Domain.Entities.Workouts;
using FitCoachPro.Domain.Entities.Workouts.Plans;

namespace FitCoachPro.Application.Common.Extensions;

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
           exercise.GifUrl,
           exercise.RowVersion);

    public static IReadOnlyList<ExerciseModel> ToModel(this IReadOnlyList<Exercise> exercise) =>
        exercise
            .Select(x => x.ToModel())
            .ToList()
            .AsReadOnly();
}
