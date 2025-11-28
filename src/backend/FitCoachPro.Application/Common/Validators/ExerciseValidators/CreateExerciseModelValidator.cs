using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.Exercise;
using FluentValidation;

namespace FitCoachPro.Application.Common.Validators.ExerciseValidators;

public class CreateExerciseModelValidator : AbstractValidator<CreateExerciseModel>
{
    public CreateExerciseModelValidator()
    {
        RuleFor(x => x.ExerciseName)
            .NotEmpty()
                .WithErrorCode(ExerciseErrors.NameEmpty.Code)
                .WithMessage(ExerciseErrors.NameEmpty.Message)
            .Length(3, 50)
                .WithErrorCode(ExerciseErrors.NameInvalid.Code)
                .WithMessage(ExerciseErrors.NameInvalid.Message);

        RuleFor(x => x.GifUrl)
            .NotEmpty()
                .WithErrorCode(ExerciseErrors.GifUrlEmpty.Code)
                .WithMessage(ExerciseErrors.GifUrlEmpty.Message)
            .Length(1, 100)
                .WithErrorCode(ExerciseErrors.GifUrlInvalid.Code)
                .WithMessage(ExerciseErrors.GifUrlInvalid.Message);
    }
}
