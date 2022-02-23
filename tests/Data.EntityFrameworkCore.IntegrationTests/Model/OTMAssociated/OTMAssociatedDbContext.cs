using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Model.OTMAssociated;

public class OTMAssociatedDbContext : DbContext
{
  public OTMAssociatedDbContext(DbContextOptions<OTMAssociatedDbContext> options)
    : base(options)
  {
  }

  public DbSet<Primary> Primaries { get; set; } = default!;
  public DbSet<Secondary> Secondaries { get; set; } = default!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Primary>(entity =>
    {
      entity
        .HasMany(x => x.Secondaries)
        .WithOne(x => x.Primary!)
        .IsRequired(false)
        .OnDelete(DeleteBehavior.SetNull);
    });
    modelBuilder.Entity<Secondary>();
  }
}
