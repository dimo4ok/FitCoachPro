using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Models.Auth;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace FitCoachPro.Application.Services;

public class AuthService(
    UserManager<User> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IUserRepository domainUserRepository,
    IJwtService jwtService,
    IUnitOfWork unitOfWork
        ) : IAuthService
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;
    private readonly IUserRepository _userRepository = domainUserRepository;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<AuthModel>> SignUpAsync(SignUpModel model, CancellationToken cancellationToken)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var existingEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingEmail != null)
                return Result<AuthModel>.Fail(UserErrors.EmailAlreadyExists, 400);

            var user = new User
            {
                Email = model.Email,
                UserName = model.UserName
            };

            var createUserResult = await _userManager.CreateAsync(user, model.Password);
            if (!createUserResult.Succeeded)
                return Result<AuthModel>.Fail(createUserResult.Errors.ToErrorList(), 400);

            var roleExists = await _roleManager.RoleExistsAsync(model.Role.ToString());
            if (!roleExists)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<AuthModel>.Fail(UserErrors.RoleNotFound, 400);
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, model.Role.ToString());
            if (!addRoleResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<AuthModel>.Fail(addRoleResult.Errors.ToErrorList(), 400);
            }

            var domainUserModel = new CreateUserModel
            {
                UserId = user.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Role = model.Role
            };

            var createDomainUserId = await _userRepository.CreateAsync(domainUserModel, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

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
            await transaction.RollbackAsync(cancellationToken);
            return Result<AuthModel>.Fail(SystemErrors.TransactionFailed, 500);
        }
    }

    public async Task<Result<AuthModel>> SignInAsync(SignInModel model, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user == null)
            return Result<AuthModel>.Fail(UserErrors.NotFound);

        var validPassword = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!validPassword)
            return Result<AuthModel>.Fail(UserErrors.InvalidCredentials, 401);

        var roleString = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
        if (string.IsNullOrEmpty(roleString))
            return Result<AuthModel>.Fail(UserErrors.RoleNotFound, 500);

        if (!Enum.TryParse<UserRole>(roleString, true, out var userRole))
            return Result<AuthModel>.Fail(UserErrors.InvalidRole, 500);

        var domainUser = await _userRepository.GetByAppUserIdAndRoleAsync(user.Id, userRole, cancellationToken);
        if (domainUser == null)
            return Result<AuthModel>.Fail(UserErrors.NotFound);

        var jwtPayloadModel = new JwtPayloadModel
        {
            Id = domainUser.Id,
            UserName = user.UserName!,
            Role = userRole,
        };

        var authModel = GenerateTokenByData(jwtPayloadModel);

        return Result<AuthModel>.Success(authModel);
    }

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
