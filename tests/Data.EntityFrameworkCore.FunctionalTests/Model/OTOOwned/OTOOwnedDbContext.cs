using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Model.OTOOwned;

public class OTOOwnedDbContext : DbContext
{
  public OTOOwnedDbContext(DbContextOptions<OTOOwnedDbContext> options)
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
        .HasOne(x => x.Secondary)
        .WithOne(x => x!.Primary!)
        .HasForeignKey<Secondary>();
    });

    modelBuilder.Entity<Secondary>();
  }
}
