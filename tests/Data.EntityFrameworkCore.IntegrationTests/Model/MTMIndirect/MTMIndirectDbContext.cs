using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Model.MTMIndirect
{
  public class MTMIndirectDbContext : DbContext
  {
    public MTMIndirectDbContext(DbContextOptions<MTMIndirectDbContext> options)
      : base(options)
    {
    }

    public DbSet<Primary> Primaries { get; set; }
    public DbSet<Secondary> Secondaries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Primary>();
      modelBuilder.Entity<Secondary>();
      modelBuilder.Entity<PrimarySecondary>(entity =>
      {
        entity.HasKey(x => new { x.PrimaryId, x.SecondaryId });

        entity
          .HasOne<Primary>()
          .WithMany(x => x.Secondaries)
          .HasForeignKey(x => x.PrimaryId);

        entity
          .HasOne<Secondary>()
          .WithMany(x => x.Primaries)
          .HasForeignKey(x => x.SecondaryId);
      });
    }
  }
}
