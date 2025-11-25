
namespace FitCoachPro.API.Exceptions
{
    internal interface IExceptionHandler
    {
        Task InvokeAsync(HttpContext context);
    }
}