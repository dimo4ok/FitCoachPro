using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Response;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FitCoachPro.API.Exceptions;

internal sealed class GlobalExceptionHandler(RequestDelegate next) : IExceptionHandler
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        Result result;

        if (ex is DbUpdateConcurrencyException)
            result = Result.Fail(SystemErrors.ConcurrencyConflict, 409);
        else
            result = Result.Fail(ex.ToError(), 500);

        context.Response.StatusCode = result.StatusCode;

        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }
}
