using FitCoachPro.Domain.Entities.Workouts.Plans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitCoachPro.Infrastructure.Persistence.EntityConfigurations.WorkoutConfigurations.PlanConfigurations;

public class WorkoutPlanEntityConfiguration : IEntityTypeConfiguration<WorkoutPlan>
{
    public void Configure(EntityTypeBuilder<WorkoutPlan> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.WorkoutDate)
            .IsRequired();

        builder.HasIndex(x => new {x.ClientId, x.WorkoutDate})
           .IsUnique();

        builder.HasOne(x => x.Client)
            .WithMany(x => x.WorkoutPlans)
            .HasForeignKey(x => x.ClientId)
            .IsRequired();

        builder.HasMany(x => x.WorkoutItems)
            .WithOne(x => x.WorkoutPlan)
            .HasForeignKey(x => x.WorkoutPlanId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
