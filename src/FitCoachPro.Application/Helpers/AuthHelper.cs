using FitCoachPro.Application.Common.Models.Auth;
using FitCoachPro.Application.Common.Options;
using FitCoachPro.Application.Interfaces.Helpers;
using FitCoachPro.Application.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace FitCoachPro.Application.Helpers;

public class AuthHelper(
    IJwtService jwtService,
    IOptions<JwtOptions> jwtOptions
    ) : IAuthHelper
{
    private readonly IJwtService _jwtService = jwtService;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public AuthModel GenerateTokenByData(JwtPayloadModel model)
    {
        var token = _jwtService.GenerateJWT(model);

        return new AuthModel
        {
            Token = token,
            Expires = DateTime.UtcNow.AddSeconds(_jwtOptions.ExpirationSeconds),
            Id = model.Id,
            UserName = model.UserName,
            Role = model.Role
        };
    }
}
