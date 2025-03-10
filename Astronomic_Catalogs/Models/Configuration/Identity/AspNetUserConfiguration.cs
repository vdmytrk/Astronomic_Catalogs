using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Astronomic_Catalogs.Models.Configuration.Identity;

public class AspNetUserConfiguration : IEntityTypeConfiguration<AspNetUser>
{
    public void Configure(EntityTypeBuilder<AspNetUser> builder)
    {
        builder.HasMany(u => u.UserClaims)
            .WithOne(rc => rc.User)
            .HasForeignKey(rc => rc.UserId);
        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId);
        builder.HasMany(u => u.UserLogins)
            .WithOne(rc => rc.User)
            .HasForeignKey(rc => rc.UserId);
        builder.HasMany(u => u.UserTokens)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId);

        builder.Property(u => u.RegistrationDate).HasDefaultValueSql("SYSDATETIME()");
        builder.Property(u => u.CountRegisterEmailSent).HasDefaultValue(1); // Since one email is automatically sent upon registration.

    }
}
