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
using Microsoft.Extensions.Logging;

namespace FitCoachPro.Application.Commands.Auth.SignUp;

public class SignUpCommandHandler(
    UserManager<User> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IUserRepository domainUserRepository,
    IUnitOfWork unitOfWork,
    IAuthHelper authHelper,
    ILogger<SignUpCommandHandler> logger
    ) : ICommandHandler<SignUpCommand, Result<AuthModel>>
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;
    private readonly IUserRepository _userRepository = domainUserRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAuthHelper _authHelper = authHelper;
    private readonly ILogger<SignUpCommandHandler> _logger = logger;

    public async Task<Result<AuthModel>> ExecuteAsync(SignUpCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("SignUp attempt for Email: {Email}, UserName: {UserName}, Role: {Role}",
            command.Model.Email, command.Model.UserName, command.Model.Role);

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var existingEmail = await _userManager.FindByEmailAsync(command.Model.Email);
            if (existingEmail != null)
            {
                _logger.LogWarning("SignUp failed: Email {Email} already exists", command.Model.Email);
                return Result<AuthModel>.Fail(UserErrors.EmailAlreadyExists, StatusCodes.Status400BadRequest);
            }

            var user = new User
            {
                Email = command.Model.Email,
                UserName = command.Model.UserName
            };

            var createUserResult = await _userManager.CreateAsync(user, command.Model.Password);
            if (!createUserResult.Succeeded)
            {
                _logger.LogWarning("SignUp failed: Identity creation errors for {Email}. Errors: {@IdentityErrors}",
                    command.Model.Email, createUserResult.Errors);
                return Result<AuthModel>.Fail(createUserResult.Errors.ToErrorList(), StatusCodes.Status400BadRequest);
            }

            var roleExists = await _roleManager.RoleExistsAsync(command.Model.Role.ToString());
            if (!roleExists)
            {
                _logger.LogError("SignUp failed: Role {Role} does not exist in the system", command.Model.Role);

                await transaction.RollbackAsync(cancellationToken);
                return Result<AuthModel>.Fail(UserErrors.RoleNotFound, StatusCodes.Status400BadRequest);
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, command.Model.Role.ToString());
            if (!addRoleResult.Succeeded)
            {
                _logger.LogError("SignUp failed: Could not assign role {Role} to user {Email}. Errors: {@RoleErrors}",
                    command.Model.Role, command.Model.Email, addRoleResult.Errors);

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

            _logger.LogInformation("User created successfully. IdentityId: {IdentityId}, DomainId: {DomainId}",
                user.Id, createDomainUserId);

            var jwtPayloadModel = new JwtPayloadModel
            {
                Id = createDomainUserId,
                UserName = command.Model.UserName,
                Role = command.Model.Role
            };

            var authModel = _authHelper.GenerateTokenByData(jwtPayloadModel);

            _logger.LogInformation("User registration completed successfully. UserName: {UserName}, DomainId: {DomainId}, Role: {Role}, Expires: {Expiration}",
                command.Model.UserName, createDomainUserId, command.Model.Role, authModel.Expires);

            return Result<AuthModel>.Success(authModel, StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SignUp transaction failed for {Email} due to an unexpected error", command.Model.Email);

            await transaction.RollbackAsync(cancellationToken);
            return Result<AuthModel>.Fail(SystemErrors.TransactionFailed, StatusCodes.Status500InternalServerError);
        }
    }
}
