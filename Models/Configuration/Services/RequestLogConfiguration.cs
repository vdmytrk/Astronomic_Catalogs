using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Astronomic_Catalogs.Models.Services;

namespace Astronomic_Catalogs.Models.Configuration.Services;

public class RequestLogConfiguration : IEntityTypeConfiguration<RequestLog>
{
    public void Configure(EntityTypeBuilder<RequestLog> builder)
    {
        builder.ToTable("RequestLogs");

        builder.HasKey(rl => rl.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(rl => rl.RequestTime).HasDefaultValueSql("SYSDATETIME()");
        
    }
}
