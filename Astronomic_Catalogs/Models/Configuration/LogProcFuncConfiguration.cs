using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Astronomic_Catalogs.Models.Configuration;

public class LogProcFuncConfiguration : IEntityTypeConfiguration<LogProcFunc>
{
    public void Configure(EntityTypeBuilder<LogProcFunc> builder)
    {
        builder.ToTable("LogProcFunc");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.Time)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
