using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Model.MTM
{
  public class MTMDbContext : DbContext
  {
    public MTMDbContext(DbContextOptions<MTMDbContext> options)
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
