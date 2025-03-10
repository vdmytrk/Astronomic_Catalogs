using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Astronomic_Catalogs.Models.Configuration.Identity;

public class AspNetRoleConfiguration : IEntityTypeConfiguration<AspNetRole>
{
    public void Configure(EntityTypeBuilder<AspNetRole> builder)
    {
        builder.HasMany(r => r.RoleClaims)
            .WithOne(rc => rc.Role)
            .HasForeignKey(rc => rc.RoleId);
        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId);
    }
}
