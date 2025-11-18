using FitCoachPro.Application.Common.Models.Exercise;
using FitCoachPro.Domain.Entities.Workouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Application.Common.Models.WorkoutItem;

public record CreateWorkoutItemModel(string Description, Guid ExerciseId);
