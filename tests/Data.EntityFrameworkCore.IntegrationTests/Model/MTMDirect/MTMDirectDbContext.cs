using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Model.MTMDirect
{
  public class MTMDirectDbContext : DbContext
  {
    public MTMDirectDbContext(DbContextOptions<MTMDirectDbContext> options)
      : base(options)
    {
    }

    public DbSet<Primary> Primaries { get; set; }
    public DbSet<Secondary> Secondaries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Primary>();
      modelBuilder.Entity<Secondary>();
    }
  }
}
