using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Response;
using Microsoft.EntityFrameworkCore;

namespace FitCoachPro.API.Exceptions;

internal sealed class GlobalExceptionHandler(
    RequestDelegate next, 
    ILogger<GlobalExceptionHandler> logger
    ) : IExceptionHandler
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled Exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        Result result = ex switch
        {
            DbUpdateConcurrencyException => Result.Fail(SystemErrors.ConcurrencyConflict, 409),
            _ => Result.Fail(ex.ToError(), 500)
        };

        context.Response.StatusCode = result.StatusCode;

        await context.Response.WriteAsJsonAsync(result);
    }
}
