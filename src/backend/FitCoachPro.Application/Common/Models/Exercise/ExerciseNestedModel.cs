using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Application.Common.Models.Exercise;

public record ExerciseNestedModel(Guid Id, string ExerciseName, string GifUrl);

