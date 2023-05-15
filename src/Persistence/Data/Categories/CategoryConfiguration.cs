using Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Categories;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<CategoryData>
{
    public void Configure(EntityTypeBuilder<CategoryData> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .HasConversion(id => id.Value, value => new CategoryId(value));
        builder.Property(c => c.ParentId)
            .HasConversion(id => id!.Value, value => new CategoryId(value))
            .IsRequired(false);
        builder.Property(c => c.Name)
            .HasColumnType("VARCHAR(255)")
            .HasMaxLength(255)
            .IsRequired();
        builder.HasIndex(c => c.Name)
            .IsUnique();
        builder.HasOne(c => c.Parent)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentId);
    }
}
