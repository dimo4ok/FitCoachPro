using FitCoachPro.Application.Common.Models.Auth;
using FitCoachPro.Application.Interfaces.Helpers;
using FitCoachPro.Application.Interfaces.Services;

namespace FitCoachPro.Application.Helpers;

public class AuthHelper(IJwtService jwtService) : IAuthHelper
{
    private readonly IJwtService _jwtService = jwtService;

    public AuthModel GenerateTokenByData(JwtPayloadModel model)
    {
        var token = _jwtService.GenerateJWT(model);

        return new AuthModel
        {
            Token = token,
            Expires = DateTime.UtcNow.AddHours(1),
            Id = model.Id,
            UserName = model.UserName,
            Role = model.Role
        };
    }
}
