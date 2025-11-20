using FitCoachPro.Application.Common.Response;
using FluentValidation.Results;

namespace FitCoachPro.Application.Common.Extensions;

public static class ErrorExtensions
{
    public static Error ToError(this Exception ex)
    {
        return new Error($"Exception.{ex.GetType().Name}", ex.Message);
    }

    public static Error ToError(this ValidationFailure failures)
    {
        return new Error(failures.ErrorCode ?? failures.PropertyName ?? "ValidationError", failures.ErrorMessage);
    }

    public static List<Error> ToErrorList(this IEnumerable<ValidationFailure> failures)
    {
        return failures.Select(x => x.ToError()).Distinct().ToList();
    }
}
