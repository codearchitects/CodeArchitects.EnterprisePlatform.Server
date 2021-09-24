using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Model.OTOOwned
{
  public class OTOOwnedDbContext : DbContext
  {
    public OTOOwnedDbContext(DbContextOptions<OTOOwnedDbContext> options)
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
          .HasOne(x => x.Secondary)
          .WithOne(x => x.Primary)
          .HasForeignKey<Secondary>();
      });
      
      modelBuilder.Entity<Secondary>();
    }
  }
}
