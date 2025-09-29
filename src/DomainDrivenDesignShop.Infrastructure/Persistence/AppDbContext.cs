using DomainDrivenDesignShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DomainDrivenDesignShop.Infrastructure.Persistence;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Product
        modelBuilder.Entity<Product>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Name).IsRequired().HasMaxLength(200);

            b.OwnsOne(p => p.Price, money =>
            {
                money.Property(m => m.Amount).HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency).HasMaxLength(3);
            });
        });


        // Order
        modelBuilder.Entity<Order>(b =>
        {
            b.HasKey(o => o.Id);
            b.Property(o => o.CreatedAtUtc).IsRequired();
            b.Property(o => o.Currency).IsRequired().HasMaxLength(3);
        });

        // OrderLine
        modelBuilder.Entity<OrderLine>(b =>
        {
            b.HasKey(ol => ol.Id);
            b.Property(ol => ol.Quantity).IsRequired();

            b.HasOne<Order>()
                .WithMany(o => o.Lines)
                .HasForeignKey(ol => ol.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            b.OwnsOne(ol => ol.UnitPrice, money =>
            {
                money.Property(m => m.Amount).HasColumnType("decimal(18,2)");
                money.Property(m => m.Currency).HasMaxLength(3);
            });
        });

    }
}
