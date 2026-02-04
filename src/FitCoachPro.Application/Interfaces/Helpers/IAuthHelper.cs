using FitCoachPro.Application.Common.Models.Auth;

namespace FitCoachPro.Application.Interfaces.Helpers;

public interface IAuthHelper
{
    AuthModel GenerateTokenByData(JwtPayloadModel model);
}