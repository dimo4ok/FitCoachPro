using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.Exercise;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.ExerciseValidators;

public class DeleteExerciseValidator : AbstractValidator<DeleteExerciseModel>
{
    public DeleteExerciseValidator()
    {
        RuleFor(x => x.rowVersion)
            .NotEmpty()
                .WithErrorCode(ExerciseErrors.RowVersionMissing.Code)
                .WithMessage(ExerciseErrors.RowVersionMissing.Message);
    }
}
