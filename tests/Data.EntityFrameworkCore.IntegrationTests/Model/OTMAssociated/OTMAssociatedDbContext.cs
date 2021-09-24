using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Model.OTMAssociated
{
  public class OTMAssociatedDbContext : DbContext
  {
    public OTMAssociatedDbContext(DbContextOptions<OTMAssociatedDbContext> options)
      : base(options)
    {
    }

    public DbSet<Primary> Primaries { get; set; }
    public DbSet<Secondary> Secondaries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Primary>(entity =>
      {
        entity
          .HasMany(x => x.Secondaries)
          .WithOne(x => x.Primary);
      });
      modelBuilder.Entity<Secondary>();
    }
  }
}
