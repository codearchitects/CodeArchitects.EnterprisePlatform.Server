using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Fixtures.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Fixtures;

public class TestDbContext : DbContext
{
  public static readonly MultitenancyContext MultitenancyContext = new();
  public static readonly SoftDeleteContext SoftDeleteContext = new();

  public TestDbContext(DbContextOptions options)
    : base(options)
  {
  }

  public void Seed(TenantEntity entity)
  {
    MultitenancyContext.ShouldFilter = false;
    var entry = Entry(entity);
    entry.State = EntityState.Added;
    entry.CurrentValues[TenantEntity.TenantIdPropertyName] = MultitenancyContext.TenantId;
    SaveChanges();
    ChangeTracker.Clear();
    MultitenancyContext.ShouldFilter = true;
  }

  public void Seed(SoftDeleteEntity entity, bool isDeleted)
  {
    Database.EnsureCreated();
    SoftDeleteContext.ShouldFilter = false;
    var entry = Entry(entity);
    entry.State = EntityState.Added;
    entry.CurrentValues[SoftDeleteEntity.SoftDeletePropertyName] = isDeleted;
    SaveChanges();
    ChangeTracker.Clear();
    SoftDeleteContext.ShouldFilter = true;
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseCaep(data => data
      .UseMultitenancy(new MultitenancyDescriptor(MultitenancyContext))
      .UseSoftDelete(new SoftDeleteDescriptor(SoftDeleteContext)));
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<TenantEntity>(entity =>
    {
      entity.IsMultiTenant(TenantEntity.TenantIdPropertyName);
    });

    modelBuilder.Entity<SoftDeleteEntity>(entity =>
    {
      entity.IsSoftDelete(SoftDeleteEntity.SoftDeletePropertyName);
    });
  }
}