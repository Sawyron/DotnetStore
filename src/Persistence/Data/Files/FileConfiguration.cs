using Domain.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Images;

internal class FileConfiguration : IEntityTypeConfiguration<FileData>
{
    public void Configure(EntityTypeBuilder<FileData> builder)
    {
        builder.ToTable("files");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .HasConversion(
                id => id.Value,
                value => new FileId(value));
        builder.Property(i => i.Path)
            .HasColumnType("VARCHAR(255)")
            .HasMaxLength(255)
            .IsRequired();
        builder.HasIndex(i => i.Path)
            .IsUnique();
    }
}
