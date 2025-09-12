namespace FitCoachPro.Domain.Entities
{
    public class WorkoutPlan
    {
        public Guid Id { get; protected set; }
        public DateTime DateOfDoing { get; private set; }

        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public ICollection<WorkoutItem> WorkoutItems { get; set; } = new List<WorkoutItem>();
    }
}