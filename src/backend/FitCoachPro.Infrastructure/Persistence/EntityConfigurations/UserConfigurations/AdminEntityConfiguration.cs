using FitCoachPro.Domain.Entities.Users;
using FitCoachPro.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitCoachPro.Infrastructure.Persistence.EntityConfigurations.UserConfigurations;

public class AdminEntityConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
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

        builder.HasOne<AppUser>()
            .WithOne()
            .HasForeignKey<Admin>(x => x.UserId);
    }
}
