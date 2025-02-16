using Astronomic_Catalogs.Models.Connection;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Models.Configuration.Connection;

public class TestConnectionForNLogConfiguration : IEntityTypeConfiguration<TestConnectionForNLog>
{
    public void Configure(EntityTypeBuilder<TestConnectionForNLog> builder)
    {
        builder.ToTable("TestConnectionForNLog");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.Timestamp).IsRequired();

        builder.Property(e => e.Level)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Logger)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.Message).IsRequired();

    }
}
