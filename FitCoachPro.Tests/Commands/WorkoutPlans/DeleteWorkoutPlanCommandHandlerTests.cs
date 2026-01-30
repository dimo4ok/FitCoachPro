using FitCoachPro.Application.Commands.WorkoutPlans.DeleteWorkoutPlan;
using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using FitCoachPro.Tests.TestDataFactories;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace FitCoachPro.Tests.Commands.WorkoutPlans;

public class DeleteWorkoutPlanCommandHandlerTests
{
    private readonly IUserContextService _mockUserContext;
    private readonly IWorkoutPlanRepository _mockRepository;
    private readonly IUnitOfWork _mockUnitOfWork;
    private readonly IWorkoutPlanAccessService _mockAccessService;
    private readonly DeleteWorkoutPlanCommandHandler _handler;

    public DeleteWorkoutPlanCommandHandlerTests()
    {
        _mockUserContext = Substitute.For<IUserContextService>();
        _mockRepository = Substitute.For<IWorkoutPlanRepository>();
        _mockUnitOfWork = Substitute.For<IUnitOfWork>();
        _mockAccessService = Substitute.For<IWorkoutPlanAccessService>();

        _handler = new DeleteWorkoutPlanCommandHandler(
            _mockUserContext,
            _mockRepository,
            _mockUnitOfWork,
            _mockAccessService
            );
    }

    [Theory]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Coach)]
    [InlineData(UserRole.Client)]
    public async Task ExecuteAsync_IfWorkoutPlanNotFound_ReturnsFailResult(UserRole userRole)
    {
        //Arrange
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: userRole);
        var command = WorkoutPlanTestDataFactory.GetDeleteWorkoutPlanCommand();

        _mockRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>(), track: true).Returns((WorkoutPlan?)null);

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.NotFound(nameof(WorkoutPlan)), result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);

        await _mockUnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Coach)]
    [InlineData(UserRole.Client)]
    public async Task ExecuteAsync_IfUserWithoutAccess_ReturnsFailResult(UserRole userRole)
    {
        //Arrange
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: userRole);
        var command = WorkoutPlanTestDataFactory.GetDeleteWorkoutPlanCommand();
        var workoutPlan = new WorkoutPlan { Id = command.Id, ClientId = Guid.NewGuid() };

        _mockRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>(), track: true).Returns(workoutPlan);
        
        _mockUserContext.Current.Returns(currentUser);
        _mockAccessService.HasCoachAccessToWorkoutPlan(currentUser, workoutPlan.ClientId, Arg.Any<CancellationToken>()).Returns(false);

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Forbidden, result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
        
        await _mockAccessService.Received(1).HasCoachAccessToWorkoutPlan(currentUser, workoutPlan.ClientId, Arg.Any<CancellationToken>());
        await _mockUnitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_IfAllValid_ReturnSuccessResult()
    {
        //Arrange
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: UserRole.Coach);
        var command = WorkoutPlanTestDataFactory.GetDeleteWorkoutPlanCommand();
        var workoutPlan = new WorkoutPlan { Id = command.Id, ClientId = Guid.NewGuid() };

        _mockRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>(), track: true).Returns(workoutPlan);
        _mockUserContext.Current.Returns(currentUser);
        _mockAccessService.HasCoachAccessToWorkoutPlan(currentUser, workoutPlan.ClientId, Arg.Any<CancellationToken>()).Returns(true);

        //Act
        var result = await _handler.ExecuteAsync(command, default);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);

        _mockRepository.Received(1).Delete(workoutPlan);
        await _mockUnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
