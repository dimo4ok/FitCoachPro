using FitCoachPro.Domain.Entities.Users;
using FitCoachPro.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitCoachPro.Infrastructure.Persistence.EntityConfigurations;

public class IdentityEntityConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserName).
            IsRequired();
        builder.HasIndex(x => x.UserName)
            .IsUnique();

        builder.Property(x => x.Email)
            .IsRequired();
        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.PhoneNumber);
        builder.HasIndex(x => x.PhoneNumber)
            .IsUnique();

        builder.Property(x => x.PasswordHash)
            .IsRequired();

        builder.HasOne<Coach>()
           .WithOne()
           .HasForeignKey<Coach>(x => x.UserId)
           .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Client>()
            .WithOne()
            .HasForeignKey<Client>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Admin>()
            .WithOne()
            .HasForeignKey<Admin>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
