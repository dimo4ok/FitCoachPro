using FitCoachPro.Domain.Entities.Workouts.Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitCoachPro.Infrastructure.Persistence.EntityConfigurations.WorkoutConfigurations.ItemConfigurations;

public class WorkoutItemEntityConfiguration : IEntityTypeConfiguration<WorkoutItem>
{
    public void Configure(EntityTypeBuilder<WorkoutItem> builder)
    {
        builder.HasKey(x => x.Id);

        //builder.Property(x => x.Reps)
        //    .HasDefaultValue(null);
        //builder.Property(x => x.Sets)
        //    .HasDefaultValue(null);

        builder.Property(x => x.Description)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne(x => x.WorkoutPlan)
            .WithMany(x => x.WorkoutItems)
            .HasForeignKey(x => x.WorkoutPlanId)
            .IsRequired();

        builder.HasOne(x => x.Exercise)
            .WithMany()
            .HasForeignKey(x => x.ExerciseId)
            .IsRequired();
    }
}
