using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Model.OTOAssociated
{
  public class OTOAssociatedDbContext : DbContext
  {
    public OTOAssociatedDbContext(DbContextOptions<OTOAssociatedDbContext> options)
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
