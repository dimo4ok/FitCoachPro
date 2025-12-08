namespace FitCoachPro.Application.Common.Models.Workouts.Exercise;

public record ExerciseDetailModel(Guid Id, string ExerciseName, string GifUrl, byte[] RowVersion);
