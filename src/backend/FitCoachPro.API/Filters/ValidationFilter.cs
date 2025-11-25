
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Response;
using FluentValidation;

namespace FitCoachPro.API.Filters;

public class ValidationFilter<T>(IServiceProvider serviceProvider) : IEndpointFilter where T : class
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var model = context.Arguments.OfType<T>().FirstOrDefault();
        if (model == null)
            return await next(context);

        var validator = _serviceProvider.GetService<IValidator<T>>();
        if(validator == null)
            return await next(context);

        var validatorResult = await validator.ValidateAsync(model);
        if (!validatorResult.IsValid)
            return Result.Fail(validatorResult.Errors.ToErrorList(), 400);

        return await next(context);
    }
}
