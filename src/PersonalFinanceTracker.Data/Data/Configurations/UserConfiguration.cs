using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceTracker.Data.Data.Entities;

namespace PersonalFinanceTracker.Data.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        var user = new AppUser
        {
            Id = "46b4340e-6500-420b-9d9e-f04a516e7f19",
            UserName = "mattietorrent@gmail.com",
            NormalizedUserName = "mattietorrent@gmail.com".ToUpper(),
            Email = "mattietorrent@gmail.com",
            NormalizedEmail = "mattietorrent@gmail.com".ToUpper(),
            FirstName = "Mattie",
            LastName = "Coder",
            PhoneNumber = "233552474843"
        };

        var passwordHash = new PasswordHasher<AppUser>().HashPassword(user, "P@ssw0rd!2");
        user.PasswordHash = passwordHash;
        builder.HasData(user);
    }
}