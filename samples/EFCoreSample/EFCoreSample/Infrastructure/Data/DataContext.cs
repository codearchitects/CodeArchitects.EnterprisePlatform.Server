using CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Associations;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;
using EFCoreSample.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace EFCoreSample.Infrastructure.Data;

public class DataContext : DbContext
{
  public DataContext(DbContextOptions options)
    : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Cart>(entity =>
    {
      entity
        .HasMany(e => e.Orders)
        .WithOne()
        .Aggregate();
    });

    modelBuilder.Entity<Order>(entity =>
    {
      entity
        .HasMany(x => x.Products)
        .WithMany();
    });

    modelBuilder.Entity<Product>(entity =>
    {
      entity.IsMultiTenant("TenantId");

      entity
        .Property(e => e.Price)
        .HasPrecision(18, 2);

      entity
        .HasData(
        new
        {
          Id = Guid.Parse("9099eaf9-1e11-43f8-aebe-7dad26e53208"),
          Name = "iPhone",
          Price = 1200.0m,
          TenantId = Guid.Parse("f2ae191d-8708-4ecb-9c53-f93b668a1744"),
          IsPublic = true
        },
        new
        {
          Id = Guid.Parse("ec5f6fff-4def-4b93-b3f6-392fb4487d39"),
          Name = "iPad",
          Price = 700.0m,
          TenantId = Guid.Parse("f2ae191d-8708-4ecb-9c53-f93b668a1744"),
          IsPublic = false
        },
        new
        {
          Id = Guid.Parse("ab73d4b5-03c3-4fc4-8135-10773df6d757"),
          Name = "AirPods",
          Price = 600.0m,
          TenantId = Guid.Parse("f2ae191d-8708-4ecb-9c53-f93b668a1744"),
          IsPublic = false
        },
        new
        {
          Id = Guid.Parse("51e2887c-8442-48db-8e27-026638692b7e"),
          Name = "Dell Precision",
          Price = 4000.0m,
          TenantId = Guid.Parse("26b27fd0-6507-42e1-9674-b777136d2256"),
          IsPublic = true
        },
        new
        {
          Id = Guid.Parse("2fedacbf-bdd0-4083-8c98-8b97f6257663"),
          Name = "Dell XPS 15",
          Price = 2300.0m,
          TenantId = Guid.Parse("26b27fd0-6507-42e1-9674-b777136d2256"),
          IsPublic = false
        },
        new
        {
          Id = Guid.Parse("d9ee475d-eec4-4731-a638-37c4c36fca58"),
          Name = "Dell Monitor",
          Price = 800.0m,
          TenantId = Guid.Parse("26b27fd0-6507-42e1-9674-b777136d2256"),
          IsPublic = false
        });
    });
  }
}
