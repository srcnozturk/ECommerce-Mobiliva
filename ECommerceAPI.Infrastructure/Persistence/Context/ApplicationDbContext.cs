using ECommerceAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
    {
    }
    public DbSet<ProductEntity> Products { get; set; }
    public DbSet<OrderEntity> Orders { get; set; }
    public DbSet<OrderDetailEntity> OrderDetails { get; set; }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is ProductEntity && e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            ((ProductEntity)entry.Entity).UpdateDate = DateTime.Now;
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
