using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.Workouts.Exercise;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.ExerciseValidators;

public class UpdateExerciseModelValidatior : AbstractValidator<UpdateExerciseModel>
{
    public UpdateExerciseModelValidatior()
    {
        RuleFor(x => x.ExerciseName)
            .NotEmpty()
             .Length(3, 50);

        RuleFor(x => x.GifUrl)
            .NotEmpty()
            .Length(1, 100);
    }
}