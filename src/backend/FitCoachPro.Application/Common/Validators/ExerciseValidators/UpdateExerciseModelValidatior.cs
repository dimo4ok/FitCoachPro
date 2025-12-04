using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.Exercise;
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

        RuleFor(x => x.RowVersion)
            .NotNull();
    }
}