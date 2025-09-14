using FitCoachPro.Domain.Entities.Enums;
using FitCoachPro.Domain.Entities.Workouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Domain.Entities.Users
{
    public class Coach : User
    {
        public override UserRole Role { get; protected set; } = UserRole.Coach;

        public ICollection<Client> Clients { get; set; } = new List<Client>();

        public ICollection<TemplateWorkoutItem> TemplateWorkoutItems { get; set; } = new List<TemplateWorkoutItem>();
    }
}
