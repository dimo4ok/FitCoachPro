using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Domain.Entities
{
    public class Exercise
    {
        public Guid Id { get; set; }
        public string ExerciseName { get; set; } = null!;
        public string GifUrl { get; set; } = null!;
    }
}
