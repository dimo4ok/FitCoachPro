using FitCoachPro.Application.Common.Models;

namespace FitCoachPro.Application.Interfaces.Services;

public interface IJwtService
{
    public string GenerateJWT(JwtPayloadModel jwtUserModel);
}
