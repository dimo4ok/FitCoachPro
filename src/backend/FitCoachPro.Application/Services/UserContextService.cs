using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Models;
using FitCoachPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace FitCoachPro.Application.Services;

public class UserContextService(IHttpContextAccessor httpContextAccessor) : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public UserContext Current => _httpContextAccessor.HttpContext?.User is { Identity.IsAuthenticated: true}
        ? _httpContextAccessor.HttpContext.User.ToUserContext()
        : throw new Exception("User not authenticated");
}
