using DomainDrivenDesignShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainDrivenDesignShop.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever();

        builder.Property(o => o.CreatedAtUtc)
               .IsRequired();

        builder.Property(o => o.Currency)
               .IsRequired()
               .HasMaxLength(3);

        builder.OwnsMany(o => o.Lines, lineBuilder =>
        {
            lineBuilder.ToTable("OrderLines");

            lineBuilder.WithOwner()
                       .HasForeignKey(l => l.OrderId);

            lineBuilder.HasKey(l => l.Id);
            lineBuilder.Property(l => l.Id).ValueGeneratedNever();

            lineBuilder.Property(l => l.ProductId)
                       .IsRequired();

            lineBuilder.Property(l => l.Quantity)
                       .IsRequired();

            lineBuilder.OwnsOne(l => l.UnitPrice, moneyBuilder =>
            {
                moneyBuilder.WithOwner();

                moneyBuilder.Property(m => m.Amount)
                            .HasPrecision(18, 2)
                            .IsRequired();

                moneyBuilder.Property(m => m.Currency)
                            .HasMaxLength(3)
                            .IsRequired();
            });
        });

        builder.Navigation(o => o.Lines)
               .HasField("_lines")
               .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
