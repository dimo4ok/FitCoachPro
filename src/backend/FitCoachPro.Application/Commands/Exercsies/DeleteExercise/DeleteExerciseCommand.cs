using FitCoachPro.Application.Common.Models.Workouts.Exercise;

namespace FitCoachPro.Application.Commands.Exercsies.DeleteExercise;

public record DeleteExerciseCommand(Guid Id, DeleteExerciseModel Model);