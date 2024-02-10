using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PersonalFinanceTracker.Data.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id = "1385990d8b8f4be9aac34606ceb76056",
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            },
            new IdentityRole
            {
                Id = "6f1072ca96c145cb83f0036c2562195b",
                Name = "Super Administrator",
                NormalizedName = "SUPER ADMINISTRATOR"
            },
            new IdentityRole
            {
                Id = "e1e5b8e43bd64a2b93924c1e4bf28af2",
                Name = "User",
                NormalizedName = "USER"
            }
        );
    }
}