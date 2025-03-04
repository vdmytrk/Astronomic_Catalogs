using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Astronomic_Catalogs.Models.Services;

namespace Astronomic_Catalogs.Models.Configuration.Services;

public class UsersLogConfiguration : IEntityTypeConfiguration<UserLog>
{
    public void Configure(EntityTypeBuilder<UserLog> builder)
    {
        builder.ToTable("UserLogs");

        builder.HasKey(rl => rl.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(rl => rl.RequestTimeUtc).HasDefaultValueSql("SYSDATETIME()");

    }
}
