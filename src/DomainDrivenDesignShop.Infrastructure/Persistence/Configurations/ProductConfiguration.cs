using DomainDrivenDesignShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainDrivenDesignShop.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Name)
               .IsRequired()
               .HasMaxLength(200);

        builder.OwnsOne(p => p.Price, priceBuilder =>
        {
            priceBuilder.WithOwner();

            priceBuilder.Property(m => m.Amount)
                         .HasPrecision(18, 2)
                         .IsRequired();

            priceBuilder.Property(m => m.Currency)
                         .HasMaxLength(3)
                         .IsRequired();
        });
    }
}
