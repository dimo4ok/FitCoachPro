using FitCoachPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCoachPro.Infrastructure.Persistence.EntityConfigurations
{
    public class WorkoutItemEntityConfiguration : IEntityTypeConfiguration<WorkoutItem>
    {
        public void Configure(EntityTypeBuilder<WorkoutItem> builder)
        {
            builder.HasKey(x => x.Id);

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
}
