using FitCoachPro.Application.Common.Response;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace FitCoachPro.Application.Common.Extensions;

public static class ErrorExtensions
{
    public static Error ToError(this IdentityError error) =>
        new(error.Code, error.Description);

    public static Error ToError(this Exception ex) =>
        new($"Exception.{ex.GetType().Name}", ex.Message);

    public static Error ToError(this ValidationFailure failures) =>
        new(failures.ErrorCode ?? failures.PropertyName ?? "ValidationError", failures.ErrorMessage);

    public static List<Error> ToErrorList(this IEnumerable<IdentityError> errors) =>
        errors
            .Select(x => x.ToError())
            .ToList();


    public static List<Error> ToErrorList(this IEnumerable<ValidationFailure> failures) =>
        failures
            .Select(x => x.ToError())
            .Distinct()
            .ToList();
}
