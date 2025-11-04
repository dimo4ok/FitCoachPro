using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.Users;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repository;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Infrastructure.Repositories;

namespace FitCoachPro.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IAppUserService _appUserService;
    private readonly IDomainUserRepository _domainUserRepository;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService
        (
        IAppUserService appUserService,
        IDomainUserRepository domainUserRepository,
        IJwtService jwtService,
        IUnitOfWork unitOfWork
        )
    {
        _appUserService = appUserService;
        _domainUserRepository = domainUserRepository;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthModel>> SignUpAsync(SignUpModel model, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var createAppUser = await _appUserService.CreateAsync(model);
            if (!createAppUser.IsSuccess)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<AuthModel>.FromResult(createAppUser);
            }

            var domainUserModel = new CreateDomainUserModel
            {
                UserId = createAppUser.Data,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Role = model.Role
            };

            var createDomainUserId = await _domainUserRepository.CreateAsync(domainUserModel, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var jwtPayloadModel = new JwtPayloadModel
            {
                Id = createDomainUserId,
                UserName = model.UserName,
                Role = model.Role
            };

            var authModel = GenerateTokenByData(jwtPayloadModel);

            return Result<AuthModel>.Success(authModel);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<AuthModel>.Fail(SystemErrors.TransactionFailed);
        }

    }

    public async Task<Result<AuthModel>> SignInAsync(SignInModel model, CancellationToken cancellationToken = default)
    {
        var authAppUser = await _appUserService.AuthenticateAsync(model);
        if (!authAppUser.IsSuccess)
            return Result<AuthModel>.FromResult(authAppUser);

        var domainUser = await _domainUserRepository.GetByAppUserIdAndRoleAsync(authAppUser.Data!.Id, authAppUser.Data.Role, cancellationToken);

        var jwtPayloadModel = new JwtPayloadModel
        {
            Id = domainUser!.Id,
            UserName = authAppUser.Data.UserName,
            Role = authAppUser.Data.Role,
        };

        var authModel = GenerateTokenByData(jwtPayloadModel);

        return Result<AuthModel>.Success(authModel);
    }

    //RefreshTokenAsync???

    private AuthModel GenerateTokenByData(JwtPayloadModel model)
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
