using FitCoachPro.Application.Common.Models.Response;
using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Users;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FitCoachPro.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAppUserService _appUserService;
        private readonly IJwtService _jwtService;

        public AuthService(IAppUserService appUserService, IJwtService jwtService)
        {
            _appUserService = appUserService;
            _jwtService = jwtService;
        }

        //Error Handling
        //Response when errors
        //AutoMapper
        //Mediator
        public async Task<Result<AuthModel>> RegisterUserAsync(CreateUserModel model, CancellationToken cancellationToken)
        {
            User domainUser = model.Role switch
            {
                UserRole.Admin => new Admin { FirstName = model.FirstName, LastName = model.LastName },
                UserRole.Coach => new Coach { FirstName = model.FirstName, LastName = model.LastName },
                UserRole.Client => new Client { FirstName = model.FirstName, LastName = model.LastName },
                _ => throw new Exception("Ivalid role!")
            };

            var userResult = await _appUserService.CreateUserAsync(domainUser, model);
            if (!userResult.IsSuccess)
                return Result<AuthModel>.FromResult(userResult);

            var authModel = GenerateTokenByData(domainUser.Id, domainUser.Role, model.UserName);

            return Result<AuthModel>.Success(authModel, HttpType.POST);
        }

        public async Task<Result<AuthModel>> LoginUserAsync(LoginUserModel model, CancellationToken cancellationToken = default)
        {
            var userResult = await _appUserService.AuthenticateUserAsync(model, cancellationToken);
            if (!userResult.IsSuccess)
                return Result<AuthModel>.FromResult(userResult);

            var userModel = userResult.Data;
            var authModel = GenerateTokenByData(userModel.Id, userModel.Role, userModel.UserName);

            return Result<AuthModel>.Success(authModel, HttpType.POST);
        }

        //RefreshTokenAsync
        //RevokeTokenAsync


        private AuthModel GenerateTokenByData(Guid id, UserRole role, string userName)
        {
            var token = _jwtService.GenerateJWT(new JwtUserModel { Id = id, Role = role, UserName = userName });

            return new AuthModel
            {
                Token = token,
                Expires = DateTime.UtcNow.AddHours(1),
                UserId = id,
                UserName = userName,
                Role = role
            };
        }
    }
}
