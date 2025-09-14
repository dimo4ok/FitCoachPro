using FitCoachPro.Domain.Entities.Users;

namespace FitCoachPro.Domain.Entities.Workouts
{
    public class WorkoutPlan
    {
        public Guid Id { get; set; }
        public DateTime DateOfDoing { get; set; }

        public Guid ClientId { get; set; }
        public Client Client { get; set; } = null!;
        public ICollection<WorkoutItem> WorkoutItems { get; set; } = new List<WorkoutItem>();
    }
}