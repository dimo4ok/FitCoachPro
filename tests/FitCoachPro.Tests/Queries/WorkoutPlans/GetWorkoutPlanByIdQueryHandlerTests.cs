using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Interfaces.Services.Access;
using FitCoachPro.Application.Queries.WorkoutPlans.GetWorkoutPlanById;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using FitCoachPro.Tests.TestDataFactories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace FitCoachPro.Tests.Queries.WorkoutPlans;

public class GetWorkoutPlanByIdQueryHandlerTests
{
    private readonly IUserContextService _mockUserContext;
    private readonly IWorkoutPlanRepository _mockRepository;
    private readonly IWorkoutPlanAccessService _mockAccessService;
    private readonly GetWorkoutPlanByIdQueryHandler _handler;

    public GetWorkoutPlanByIdQueryHandlerTests()
    {
        _mockUserContext = Substitute.For<IUserContextService>();
        _mockRepository = Substitute.For<IWorkoutPlanRepository>();
        _mockAccessService = Substitute.For<IWorkoutPlanAccessService>();

        _handler = new GetWorkoutPlanByIdQueryHandler(
            _mockUserContext,
            _mockRepository,
            _mockAccessService,
            NullLogger<GetWorkoutPlanByIdQueryHandler>.Instance
            );
    }

    [Fact]
    public async Task ExecuteAsync_IfWorkoutPlanNotFound_ReturnsFailResult()
    {
        //Arrange
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: UserRole.Admin);
        var query = WorkoutPlanTestDataFactory.GetWorkoutPlanByIdQuery();

        _mockUserContext.Current.Returns(currentUser);
        _mockRepository.GetByIdAsync(Arg.Is(query.Id), Arg.Any<CancellationToken>()).Returns((WorkoutPlan?)null);

        //Act
        var result = await _handler.ExecuteAsync(query, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.NotFound(nameof(WorkoutPlan)), result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Theory]
    [InlineData(UserRole.Coach)]
    [InlineData(UserRole.Client)]
    public async Task ExecuteAsync_IfUserWithoutAccess_ReturnsFailResult(UserRole userRole)
    {
        //Arrange
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: userRole);
        var query = WorkoutPlanTestDataFactory.GetWorkoutPlanByIdQuery();
        var workoutPlan = new WorkoutPlan();

        _mockUserContext.Current.Returns(currentUser);
        _mockRepository.GetByIdAsync(Arg.Is(query.Id), Arg.Any<CancellationToken>()).Returns(workoutPlan);
        _mockAccessService.HasUserAccessToWorkoutPlanAsync(Arg.Is(currentUser), Arg.Is(workoutPlan.ClientId), Arg.Any<CancellationToken>()).Returns(false);

        //Act
        var result = await _handler.ExecuteAsync(query, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Forbidden, result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);

        await _mockAccessService.Received(1).HasUserAccessToWorkoutPlanAsync(Arg.Is(currentUser), Arg.Is(workoutPlan.ClientId), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Coach)]
    [InlineData(UserRole.Client)]
    public async Task ExecuteAsync_IfAllValid_ReturnsSuccessResult(UserRole userRole)
    {
        //Arrange
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: userRole);
        var query = WorkoutPlanTestDataFactory.GetWorkoutPlanByIdQuery();
        var workoutPlan = new WorkoutPlan();

        _mockUserContext.Current.Returns(currentUser);
        _mockRepository.GetByIdAsync(Arg.Is(query.Id), Arg.Any<CancellationToken>()).Returns(workoutPlan);
        _mockAccessService.HasUserAccessToWorkoutPlanAsync(Arg.Is(currentUser), Arg.Is(workoutPlan.ClientId), Arg.Any<CancellationToken>()).Returns(true);

        //Act
        var result = await _handler.ExecuteAsync(query, default);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);

        await _mockAccessService.Received(1).HasUserAccessToWorkoutPlanAsync(Arg.Is(currentUser), Arg.Is(workoutPlan.ClientId), Arg.Any<CancellationToken>());
    }
}