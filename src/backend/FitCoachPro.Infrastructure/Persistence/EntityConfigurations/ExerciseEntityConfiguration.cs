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
}
