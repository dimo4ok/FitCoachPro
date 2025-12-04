using FitCoachPro.Application.Common.Models.Exercise;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.ExerciseValidators;

public class DeleteExerciseValidator : AbstractValidator<DeleteExerciseModel>
{
    public DeleteExerciseValidator()
    {
        RuleFor(x => x.RowVersion)
            .NotEmpty();
    }
}
