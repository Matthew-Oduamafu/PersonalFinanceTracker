using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PersonalFinanceTracker.Data.Data.Configurations;

public class UserRolesConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder.HasData(
            new IdentityUserRole<string>
            {
                RoleId = "6f1072ca96c145cb83f0036c2562195b",
                UserId = "71538c31f99f42078133f5e4c249d6f1"
            },
            new IdentityUserRole<string>
            {
                RoleId = "1385990d8b8f4be9aac34606ceb76056",
                UserId = "71538c31f99f42078133f5e4c249d6f1"
            },
            new IdentityUserRole<string>
            {
                RoleId = "e1e5b8e43bd64a2b93924c1e4bf28af2",
                UserId = "71538c31f99f42078133f5e4c249d6f1"
            }
        );
    }
}