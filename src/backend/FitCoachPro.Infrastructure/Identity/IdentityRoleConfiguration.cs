using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitCoachPro.Infrastructure.Identity;

public class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.HasData(
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Coach",
                NormalizedName = "COACH"
            },
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Client",
                NormalizedName = "CLIENT"
            });
    }
}
