using FitCoachPro.Application.Common.Models.Users;

namespace FitCoachPro.Application.Interfaces.Services;

public interface IJwtService
{
    public string GenerateJWT(JwtPayloadModel jwtUserModel);
}
