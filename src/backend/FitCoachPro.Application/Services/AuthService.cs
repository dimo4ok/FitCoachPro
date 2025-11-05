using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Repository;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Identity;
using FitCoachPro.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace FitCoachPro.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IDomainUserRepository _domainUserRepository;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService
        (
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IDomainUserRepository domainUserRepository,
        IJwtService jwtService,
        IUnitOfWork unitOfWork
        )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _domainUserRepository = domainUserRepository;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthModel>> SignUpAsync(SignUpModel model, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

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
                return Result<AuthModel>.Fail(MapErrors(createUserResult.Errors), 400);

            var roleExists = await _roleManager.RoleExistsAsync(model.Role.ToString());
            if (!roleExists)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<AuthModel>.Fail(UserErrors.RoleNotFound, 400);
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, model.Role.ToString());
            if (!addRoleResult.Succeeded)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<AuthModel>.Fail(MapErrors(addRoleResult.Errors), 400);
            }

            var domainUserModel = new CreateDomainUserModel
            {
                UserId = user.Id,
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
        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user == null)
            return Result<AuthModel>.Fail(UserErrors.NotFound);

        var validPassword = await _userManager.CheckPasswordAsync(user, model.Password);
        if (!validPassword)
            return Result<AuthModel>.Fail(UserErrors.WrongPassword);

        var roleString = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
        if (string.IsNullOrEmpty(roleString))
            return Result<AuthModel>.Fail(UserErrors.RoleNotFound);

        if (!Enum.TryParse<UserRole>(roleString, true, out var userRole))
            return Result<AuthModel>.Fail(UserErrors.InvalidRole);

        var domainUser = await _domainUserRepository.GetByAppUserIdAndRoleAsync(user.Id, userRole, cancellationToken);
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

    private List<Error> MapErrors(IEnumerable<IdentityError> identityErrors)
    {
        return identityErrors
            .Select(x => new Error(x.Code, x.Description))
            .ToList();
    }
}
