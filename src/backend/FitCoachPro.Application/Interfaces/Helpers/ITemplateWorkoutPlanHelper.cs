using FitCoachPro.Application.Common.Models.Workouts.TemplateWorkoutItem;
using FitCoachPro.Application.Common.Response;
using FitCoachPro.Domain.Entities.Workouts.Items;

namespace FitCoachPro.Application.Interfaces.Helpers;

public interface ITemplateWorkoutPlanHelper
{
    void SyncItems(ICollection<TemplateWorkoutItem> currentItems, IEnumerable<UpdateTemplateWorkoutItemModel> newItems);

    (bool, Error?) ExercisesExist(IEnumerable<CreateTemplateWorkoutItemModel> newItems, HashSet<Guid> exercisIdsSet);
    (bool, Error?) ValidateUpdateItems(ICollection<TemplateWorkoutItem> currentItems, IEnumerable<UpdateTemplateWorkoutItemModel> newItems, HashSet<Guid> exercisIdsSet);
}