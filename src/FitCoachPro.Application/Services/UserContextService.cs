using FitCoachPro.Application.Common.Models;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Domain.Entities.Enums;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FitCoachPro.Application.Services;

public class UserContextService(IHttpContextAccessor httpContextAccessor) : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public UserContext Current => _httpContextAccessor.HttpContext?.User is { Identity.IsAuthenticated: true}
        ? new UserContext(
            Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            Enum.Parse<UserRole>(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role)!))
        : throw new Exception("User not authenticated");
}
