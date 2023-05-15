using Domain.ProductTypes.Details;
using Domain.ProductTypes.Tags.TagOptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.TagOptions;

internal class TagOptionsConfiguration : IEntityTypeConfiguration<TagOptionData>
{
    public void Configure(EntityTypeBuilder<TagOptionData> builder)
    {
        builder.ToTable("tag_options");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .HasConversion(id => id.Value, value => new TagOptionId(value));
        builder.Property(o => o.TagId)
            .HasConversion(id => id.Value, value => new TagId(value));
        builder.Property(o => o.Value)
            .HasColumnType("VARCHAR(255)")
            .HasMaxLength(255)
            .IsRequired();
        builder.HasOne(o => o.Tag)
            .WithMany(t => t.Options)
            .HasForeignKey(o => o.TagId)
            .IsRequired();
        builder.HasIndex(t => new { t.TagId, t.Value })
            .IsUnique();
    }
}
