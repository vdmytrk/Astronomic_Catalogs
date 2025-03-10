using Astronomic_Catalogs.Models.Connection;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Models.Configuration.Connection;

public class ActualDateConfiguration : IEntityTypeConfiguration<TestConnectionForNLog>
{
    public void Configure(EntityTypeBuilder<TestConnectionForNLog> builder)
    {
        builder.ToTable("DateTable");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
    }
}
