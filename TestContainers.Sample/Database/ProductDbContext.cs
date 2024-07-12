using Microsoft.EntityFrameworkCore;
using TestContainers.Sample.EntityModels;

namespace TestContainers.Sample.Database;

public class ProductDbContext:DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options):base(options)
    {
        
    }

    public DbSet<ProductEntity> Products => base.Set<ProductEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductEntity>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.ProductName).HasMaxLength(400);
            builder.Property(c => c.ProductPrice).HasPrecision(8, 2);
            builder.Property(c => c.Id).UseHiLo();
        });
    }
}