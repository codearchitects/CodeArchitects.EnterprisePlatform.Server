using CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

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
    IMultitenancyContextBypasser? bypasser = GetMultitenancyContextBypasser(_context);

    using (bypasser?.BypassMultitenancy())
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

  private static IMultitenancyContextBypasser? GetMultitenancyContextBypasser(IInfrastructure<IServiceProvider> accessor)
  {
    IServiceProvider internalServiceProvider = accessor.Instance;

    object? bypasser = internalServiceProvider.GetService(typeof(IMultitenancyContextBypasser))
      ?? internalServiceProvider.GetService<IDbContextOptions>()
          ?.Extensions.OfType<CoreOptionsExtension>().FirstOrDefault()
          ?.ApplicationServiceProvider
          ?.GetService(typeof(IMultitenancyContextBypasser));

    return bypasser as IMultitenancyContextBypasser;
  }
}
