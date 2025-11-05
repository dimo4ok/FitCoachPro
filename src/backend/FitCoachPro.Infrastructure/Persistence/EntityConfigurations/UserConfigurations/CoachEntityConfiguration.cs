using FitCoachPro.Domain.Entities.Identity;
using FitCoachPro.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitCoachPro.Infrastructure.Persistence.EntityConfigurations.UserConfigurations;

public class CoachEntityConfiguration : IEntityTypeConfiguration<Coach>
{
    public void Configure(EntityTypeBuilder<Coach> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<Coach>(x => x.UserId)
            .IsRequired();

        builder.HasMany(x => x.Clients)
            .WithOne(x => x.Coach)
            .HasForeignKey(x => x.CoachId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.TemplateWorkoutPlans)
            .WithOne(x => x.Coach)
            .HasForeignKey(x => x.CoachId)
            .IsRequired();
    }
}
