using FitCoachPro.Application.Common.Models;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Services.Access;
using FitCoachPro.Domain.Entities.Enums;
using NSubstitute;


namespace FitCoachPro.Tests.Services.Access;

public class WorkoutPlanAccessServiceTests
{
    private IUserRepository _mockRepository = Substitute.For<IUserRepository>();
    private WorkoutPlanAccessService _accessService;

    public WorkoutPlanAccessServiceTests()
    {
        _accessService = new (_mockRepository);
    }

    [Theory]
    [InlineData(UserRole.Coach)]
    [InlineData(UserRole.Client)]
    public async Task HasUserAccessToWorkoutPlanAsync_IfUserWithoutAccess_ReturnFalse(UserRole role)
    {
        //Arrange
        var currentUser = new UserContext(Guid.NewGuid(), role);
        var clientId = Guid.NewGuid();

        if (role == UserRole.Coach)
            _mockRepository.CanCoachAccessClientAsync(currentUser.UserId, clientId, Arg.Any<CancellationToken>()).Returns(false);

        //Act
        var result = await _accessService.HasUserAccessToWorkoutPlanAsync(currentUser, clientId, default);

        //Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Coach)]
    [InlineData(UserRole.Client)]
    public async Task HasUserAccessToWorkoutPlanAsync_IfUserWithAccess_ReturnTrue(UserRole role)
    {
        //Arrange
        var id = Guid.NewGuid();
        var currentUser = new UserContext(id, role);
        var clientId = id;

        if (role == UserRole.Coach)
            _mockRepository.CanCoachAccessClientAsync(currentUser.UserId, clientId, Arg.Any<CancellationToken>()).Returns(true);

        //Act
        var result = await _accessService.HasUserAccessToWorkoutPlanAsync(currentUser, clientId, default);

        //Assert
        Assert.True(result);
    }


    [Theory]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Coach)]
    [InlineData(UserRole.Client)]
    public async Task HasCoachAccessToWorkoutPlan_IfUserWithoutAccess_ReturnFalse(UserRole role)
    {
        //Arrange
        var currentUser = new UserContext(Guid.NewGuid(), role);
        var clientId = Guid.NewGuid();

        if (role == UserRole.Coach)
            _mockRepository.CanCoachAccessClientAsync(currentUser.UserId, clientId, Arg.Any<CancellationToken>()).Returns(false);

        //Act
        var result = await _accessService.HasCoachAccessToWorkoutPlan(currentUser, clientId, default);

        //Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasCoachAccessToWorkoutPlan_IfUserWithAccess_ReturnTrue()
    {
        //Arrange
        var currentUser = new UserContext(Guid.NewGuid(), UserRole.Coach);
        var clientId = Guid.NewGuid();
        
        _mockRepository.CanCoachAccessClientAsync(currentUser.UserId, clientId, Arg.Any<CancellationToken>()).Returns(true);

        //Act
        var result = await _accessService.HasCoachAccessToWorkoutPlan(currentUser, clientId, default);

        //Assert
        Assert.True(result);
    }

}
