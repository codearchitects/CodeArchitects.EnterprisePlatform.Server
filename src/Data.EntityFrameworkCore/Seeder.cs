using CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

internal class Seeder : ISeeder
{
  private readonly DbContext _context;

  public Seeder(DbContext context)
  {
    _context = context;
  }

  public void Apply(DataSeed seed)
  {
    IMultitenancyContextBypasser bypasser = _context.GetService<IMultitenancyContextBypasser>();

    using (bypasser.BypassMultitenancy())
    {
      seed.Seed(this);
      _context.SaveChanges();
      _context.ChangeTracker.Clear();
    }
  }

  public void Seed<TEntity>(IEnumerable<TEntity> entities)
    where TEntity : class
  {
    if (!_context.Set<TEntity>().AsNoMultitenancy().Any())
    {
      _context.AddRange(entities);
    }
  }
}
