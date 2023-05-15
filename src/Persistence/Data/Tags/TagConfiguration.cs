using Domain.Categories;
using Domain.ProductTypes.Details;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Tags;

internal class TagConfiguration : IEntityTypeConfiguration<TagData>
{
    public void Configure(EntityTypeBuilder<TagData> builder)
    {
        builder.ToTable("tags");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .HasConversion(id => id.Value, value => new TagId(value));
        builder.Property(t => t.CategoryId)
            .HasConversion(id => id.Value, value => new CategoryId(value));
        builder.Property(t => t.Name)
            .HasColumnType("VARCHAR(100)")
            .HasMaxLength(100)
            .IsRequired();
        builder.HasOne(t => t.Category)
            .WithMany()
            .HasForeignKey(t => t.CategoryId)
            .IsRequired();
        builder.HasIndex(t => new { t.CategoryId, t.Name })
            .IsUnique();
    }
}
