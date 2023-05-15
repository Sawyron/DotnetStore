using Domain.Categories;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Products;

internal class ProductConfiguration : IEntityTypeConfiguration<ProductData>
{
    public void Configure(EntityTypeBuilder<ProductData> builder)
    {
        builder.ToTable("products");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .HasConversion(id => id.Value, value => new ProductId(value));
        builder.Property(p => p.CategoryId)
            .HasConversion(id => id.Value, value => new CategoryId(value));
        builder.Property(p => p.Price)
            .IsRequired();
        builder.Property(p => p.Name)
            .HasColumnType("VARCHAR(100)")
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(p => p.Description)
            .HasColumnType("VARCHAR(512)")
            .HasMaxLength(512)
            .IsRequired();
        builder.Ignore(p => p.OptionIds);
        builder.HasMany(p => p.Options)
            .WithMany()
            .UsingEntity(join =>
            {
                join.ToTable("product_tag_options");
            });
        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .IsRequired();
        builder.HasOne(p => p.Photo)
            .WithOne()
            .HasForeignKey<ProductData>(p => p.PhotoId)
            .IsRequired();
        builder.HasIndex(p => new { p.CategoryId, p.Name })
            .IsUnique();
    }
}
