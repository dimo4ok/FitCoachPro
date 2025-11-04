using FitCoachPro.Domain.Entities.Workouts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitCoachPro.Infrastructure.Persistence.EntityConfigurations.WorkoutConfigurations;

public class ExerciseEntityConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ExerciseName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.GifUrl)
            .HasMaxLength(100)
            .IsRequired();
    }
}
