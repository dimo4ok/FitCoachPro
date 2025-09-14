using FitCoachPro.Domain.Entities.Workouts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Infrastructure.Persistence.EntityConfigurations.WorkoutConfigurations
{
    public class TemplateWorkoutItemEntityConfiguration : IEntityTypeConfiguration<TemplateWorkoutItem>
    {
        public void Configure(EntityTypeBuilder<TemplateWorkoutItem> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Description)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt);

            builder.HasOne(x => x.Coach)
                   .WithMany(c => c.TemplateWorkoutItems)
                   .HasForeignKey(x => x.CoachId)
                   .IsRequired();

            builder.HasOne(x => x.Exercise)
                   .WithMany()
                   .HasForeignKey(x => x.ExerciseId)
                   .IsRequired();
        }
    }
}
