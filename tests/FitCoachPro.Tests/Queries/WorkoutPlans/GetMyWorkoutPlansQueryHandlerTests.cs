using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Interfaces.Repositories;
using FitCoachPro.Application.Interfaces.Services;
using FitCoachPro.Application.Queries.WorkoutPlans.GetMyWorkoutPlans;
using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Workouts.Plans;
using FitCoachPro.Tests.TestDataFactories;
using Microsoft.AspNetCore.Http;
using MockQueryable;
using NSubstitute;

namespace FitCoachPro.Tests.Queries.WorkoutPlans;

public class GetMyWorkoutPlansQueryHandlerTests
{
    private readonly IUserContextService _mockUserContext;
    private readonly IWorkoutPlanRepository _mockRepository;
    private readonly GetMyWorkoutPlansQueryHandler _handler;

    public GetMyWorkoutPlansQueryHandlerTests()
    {
        _mockUserContext = Substitute.For<IUserContextService>();
        _mockRepository = Substitute.For<IWorkoutPlanRepository>();

        _handler = new GetMyWorkoutPlansQueryHandler(
            _mockUserContext,
            _mockRepository
            );
    }

    [Theory]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Coach)]
    public async Task ExecuteAsync_IfUserWithoutAccess_ReturnsFailResult(UserRole userRole)
    {
        //Arrange
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: userRole);
        var query = WorkoutPlanTestDataFactory.GetMyWorkoutPlansQuery();

        _mockUserContext.Current.Returns(currentUser);

        //Act
        var result = await _handler.ExecuteAsync(query, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.Forbidden, result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
    }

    [Fact]
    public async Task ExecuteAsync_IfWorkoutPlansNotFound_ReturnsFailResult()
    {
        //Arrange
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: UserRole.Client);
        var query = WorkoutPlanTestDataFactory.GetMyWorkoutPlansQuery();
        var emptyPlansQuery = new List<WorkoutPlan>().BuildMock();

        _mockUserContext.Current.Returns(currentUser);
        _mockRepository.GetAllByUserIdAsQuery(Arg.Is(currentUser.UserId)).Returns(emptyPlansQuery);

        //Act
        var result = await _handler.ExecuteAsync(query, default);

        //Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(DomainErrors.NotFound(nameof(WorkoutPlan)), result.Errors!.FirstOrDefault());
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task ExecuteAsync_IfAllValid_ReturnsSuccessResult()
    {
        //Arrange
        var currentUser = WorkoutPlanTestDataFactory.GetCurrentUser(role: UserRole.Client);
        var query = WorkoutPlanTestDataFactory.GetMyWorkoutPlansQuery();
        var plansQuery = new List<WorkoutPlan> { new(), new() }.BuildMock();

        _mockUserContext.Current.Returns(currentUser);
        _mockRepository.GetAllByUserIdAsQuery(Arg.Is(currentUser.UserId)).Returns(plansQuery);

        //Act
        var result = await _handler.ExecuteAsync(query, default);

        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Items.Count);
    }
}