using FitCoachPro.Application.Common.Models.Auth;

namespace FitCoachPro.Application.Interfaces.Services;

public interface IJwtService
{
    public string GenerateJWT(JwtPayloadModel jwtUserModel);
}
