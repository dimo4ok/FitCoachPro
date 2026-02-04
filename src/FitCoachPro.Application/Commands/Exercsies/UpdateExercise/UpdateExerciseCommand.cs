using FitCoachPro.Application.Common.Models.Workouts.Exercise;

namespace FitCoachPro.Application.Commands.Exercsies.UpdateExercise;

public record UpdateExerciseCommand(Guid Id, UpdateExerciseModel Model);