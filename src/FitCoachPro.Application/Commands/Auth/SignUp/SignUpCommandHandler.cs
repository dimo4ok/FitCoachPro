using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Extensions;
using FitCoachPro.Application.Common.Models.Auth;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Application.Interfaces.Helpers;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Mediator.Interfaces;
using FitCoachPro.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace FitCoachPro.Application.Commands.Auth.SignUp;

public class SignUpCommandHandler(
    UserManager<User> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IUserRepository domainUserRepository,
    IUnitOfWork unitOfWork,
    IAuthHelper authHelper
    ) : ICommandHandler<SignUpCommand, Result<AuthModel>>
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;
    private readonly IUserRepository _userRepository = domainUserRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAuthHelper _authHelper = authHelper;

    public async Task<Result<AuthModel>> ExecuteAsync(SignUpCommand command, CancellationToken cancellationToken)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var existingEmail = await _userManager.FindByEmailAsync(command.Model.Email);
            if (existingEmail != null)
                return Result<AuthModel>.Fail(UserErrors.EmailAlreadyExists, StatusCodes.Status400BadRequest);

            var user = new User
            {
                Email = command.Model.Email,
                UserName = command.Model.UserName
            };

            var createUserResult = await _userManager.CreateAsync(user, command.Model.Password);
            if (!createUserResult.Succeeded)
                return Result<AuthModel>.Fail(createUserResult.Errors.ToErrorList(), StatusCodes.Status400BadRequest);

            var roleExists = await _roleManager.RoleExistsAsync(command.Model.Role.ToString());
            if (!roleExists)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<AuthModel>.Fail(UserErrors.RoleNotFound, StatusCodes.Status400BadRequest);
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, command.Model.Role.ToString());
            if (!addRoleResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<AuthModel>.Fail(addRoleResult.Errors.ToErrorList(), StatusCodes.Status400BadRequest);
            }

            var domainUserModel = new CreateUserModel
            {
                UserId = user.Id,
                FirstName = command.Model.FirstName,
                LastName = command.Model.LastName,
                Role = command.Model.Role
            };

            var createDomainUserId = await _userRepository.CreateAsync(domainUserModel, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var jwtPayloadModel = new JwtPayloadModel
            {
                Id = createDomainUserId,
                UserName = command.Model.UserName,
                Role = command.Model.Role
            };

            var authModel = _authHelper.GenerateTokenByData(jwtPayloadModel);

            return Result<AuthModel>.Success(authModel, StatusCodes.Status201Created);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result<AuthModel>.Fail(SystemErrors.TransactionFailed, StatusCodes.Status500InternalServerError);
        }
    }
}
