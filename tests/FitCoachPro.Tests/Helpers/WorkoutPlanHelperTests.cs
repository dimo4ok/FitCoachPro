using FitCoachPro.Application.Common.Errors;
using FitCoachPro.Application.Common.Models.Workouts.WorkoutItem;
using FitCoachPro.Application.Helpers;
using FitCoachPro.Domain.Entities.Workouts.Items;

namespace FitCoachPro.Tests.Helpers;

public class WorkoutPlanHelperTests
{
    private readonly WorkoutPlanHelper _helper = new();

    [Fact]
    public void ExercisesExist_IfExerciseIdNotExist_ReturnsFalse()
    {
        // Arrange
        var invalidExerciseId = Guid.NewGuid();
        var existingExerciseIds = new HashSet<Guid> { Guid.NewGuid() };

        var newItems = new[]
        {
            new CreateWorkoutItemModel(1, 1, "", invalidExerciseId)
        };

        // Act
        var (success, error) = _helper.ExercisesExist(newItems, existingExerciseIds);

        // Assert
        Assert.False(success);
        Assert.NotNull(error);
    }

    [Fact]
    public void ExercisesExist_IfExerciseIdExists_ReturnsTrue()
    {
        // Arrange
        var validExerciseId = Guid.NewGuid();

        var existingExerciseIds = new HashSet<Guid> { validExerciseId };

        var newItems = new[]
        {
            new CreateWorkoutItemModel(1, 1, "", validExerciseId)
        };

        // Act
        var (success, error) = _helper.ExercisesExist(newItems, existingExerciseIds);

        // Assert
        Assert.True(success);
    }

    [Fact]
    public void ValidateUpdateItems_IfExerciseIdNotExist_ReturnsFalse()
    {
        // Arrange
        var invalidExerciseId = Guid.NewGuid();
        var existingExerciseIds = new HashSet<Guid> { Guid.NewGuid() };
        var workoutItems = new List<WorkoutItem>();

        var newItems = new[]
        {
            new UpdateWorkoutItemModel(Guid.NewGuid(), 1, 1, "", invalidExerciseId)
        };

        // Act
        var (success, error) = _helper.ValidateUpdateItems(workoutItems, newItems, existingExerciseIds);

        // Assert
        Assert.False(success);
        Assert.NotNull(error);
    }

    [Fact]
    public void ValidateUpdateItems_IfUpdateItemsNotExistInCurrentItems_ReturnsFalse()
    {
        // Arrange
        var validExerciseId = Guid.NewGuid();
        var invalidNewItemId = Guid.NewGuid();
        var existingExerciseIds = new HashSet<Guid> { validExerciseId };

        var currentItems = new[] { new WorkoutItem { Id = Guid.NewGuid() } };
        var newItems = new[]
        {
            new UpdateWorkoutItemModel(invalidNewItemId, 1, 1, "", validExerciseId)
        };

        // Act
        var (success, error) = _helper.ValidateUpdateItems(currentItems, newItems, existingExerciseIds);

        // Assert
        Assert.False(success);
        Assert.Equal(DomainErrors.InvalidEntityId(nameof(WorkoutItem)), error);
    }

    [Fact]
    public void ValidateUpdateItems_AllValid_ReturnsTrue()
    {
        // Arrange
        var validExerciseId = Guid.NewGuid();
        var invalidNewItemId = Guid.NewGuid();
        var existingExerciseIds = new HashSet<Guid> { validExerciseId };

        var currentItems = new[] { new WorkoutItem { Id = invalidNewItemId } };
        var newItems = new[]
        {
            new UpdateWorkoutItemModel(invalidNewItemId, 1, 1, "", validExerciseId)
        };

        // Act
        var (success, error) = _helper.ValidateUpdateItems(currentItems, newItems, existingExerciseIds);

        // Assert
        Assert.True(success);
    }
}
