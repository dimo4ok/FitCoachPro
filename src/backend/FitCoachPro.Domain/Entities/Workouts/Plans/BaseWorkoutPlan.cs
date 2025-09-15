using FitCoachPro.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Domain.Entities.Workouts.Plans
{
    public abstract class BaseWorkoutPlan 
    {
        public Guid Id { get; set; }
    }
}
