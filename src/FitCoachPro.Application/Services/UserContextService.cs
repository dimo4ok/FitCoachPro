using FitCoachPro.Application.Common.Models;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Domain.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace FitCoachPro.Application.Services;

public class UserContextService(
    IHttpContextAccessor httpContextAccessor,
    ILogger<UserContextService> logger
    ) : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger<UserContextService> _logger = logger;

    public UserContext Current => _httpContextAccessor.HttpContext?.User is { Identity.IsAuthenticated: true }
        ? new UserContext(
            Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            Enum.Parse<UserRole>(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!))
        : LogAndThrow();

    private UserContext LogAndThrow()
    {
        _logger.LogError("Access denied: User is not authenticated or HttpContext is null.");
        throw new UnauthorizedAccessException("User not authenticated");
    }
}