using FitCoachPro.Application.Common.Models.WorkoutItem;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Domain.Entities.Workouts.Items;

namespace FitCoachPro.Application.Interfaces.Helpers;

public interface IWorkoutPlanHelper
{
    void SyncItems(ICollection<WorkoutItem> currentItems, IEnumerable<UpdateWorkoutItemModel> newItems);

    (bool, Error?) ExercisesExist(IEnumerable<CreateWorkoutItemModel> newItems, HashSet<Guid> exercisIdsSet);
    (bool, Error?) ValidateUpdateItems(ICollection<WorkoutItem> currentItems, IEnumerable<UpdateWorkoutItemModel> newItems, HashSet<Guid> exercisIdsSet);
}