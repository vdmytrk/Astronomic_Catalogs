using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Models.Configuration;

public class NameObjectConfiguration : IEntityTypeConfiguration<NameObject>
{
    public void Configure(EntityTypeBuilder<NameObject> builder)
    {
        builder.ToTable("NameObject");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.Object)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Name)
            .HasMaxLength(6);

        builder.Property(e => e.Comment)
            .HasMaxLength(50);
    }
}
