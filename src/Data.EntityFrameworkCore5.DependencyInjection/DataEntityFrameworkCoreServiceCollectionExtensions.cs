using CodeArchitects.Platform.Data.EntityFrameworkCore;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Query;
using CodeArchitects.Platform.Data.Tracking;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Methods for adding the Entity Framework Core data context to the application services.
/// </summary>
public static class DataEntityFrameworkCoreServiceCollectionExtensions
{
  /// <summary>
  /// Injects the services needed to support the Entity Framework Core data context.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <returns>The same <see cref="IServiceCollection"/> instance.</returns>
  public static IServiceCollection AddData<TDbContext>(this IServiceCollection services)
    where TDbContext : DbContext
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));

    services.AddScoped<ITrackingContext, TrackingContext>();
    services.AddScoped<IPredicateProvider, PredicateProvider>();
    services.AddScoped<IPredicateTemplateFactory>(sp => new PredicateTemplateFactory(sp.GetRequiredService<DbContext>().Model));
    services.AddSingleton<IPredicateTemplateCache>(PredicateTemplateCache.Create());
    services.AddScoped<IStateManager<TDbContext>, StateManager<TDbContext>>();
    services.AddScoped<IDataContext<TDbContext>, DataContext<TDbContext>>();
    services.AddScoped<IDataContext>(sp => sp.GetRequiredService<IDataContext<TDbContext>>());

    return services;
  }
}
