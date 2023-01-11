using CodeArchitects.Platform.Data;
using CodeArchitects.Platform.Data.EntityFrameworkCore;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Materialization;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Query;
using CodeArchitects.Platform.Data.EntityFrameworkCore5.DependencyInjection;
using CodeArchitects.Platform.Data.Tracking;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

using IDataContext = CodeArchitects.Platform.Data.EntityFrameworkCore.IDataContext;

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

    return services.AddDataCore<TDbContext>(new EntityFrameworkCoreConfigurationBuilder());
  }

  /// <summary>
  /// Injects the services needed to support the Entity Framework Core data context.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <param name="configurationAction">An action that specifies the Entity Framework Core configuration.</param>
  /// <returns>The same <see cref="IServiceCollection"/> instance.</returns>
  public static IServiceCollection AddData<TDbContext>(this IServiceCollection services, Action<IEntityFrameworkCoreConfigurationBuilder> configurationAction)
    where TDbContext : DbContext
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));
    if (configurationAction is null)
      throw new ArgumentNullException(nameof(configurationAction));

    EntityFrameworkCoreConfigurationBuilder configurationBuilder = new();
    configurationAction(configurationBuilder);

    return services.AddDataCore<TDbContext>(configurationBuilder);
  }

  private static IServiceCollection AddDataCore<TDbContext>(this IServiceCollection services, EntityFrameworkCoreConfigurationBuilder configurationBuilder)
    where TDbContext : DbContext
  {
    if (configurationBuilder.SeedType is not null)
    {
      services.AddSingleton(typeof(DataSeed), configurationBuilder.SeedType);
    }

    services.AddScoped<ITrackingContext, TrackingContext>();

    services.AddScoped<IPredicateProvider>(sp => new PredicateProvider(sp.GetRequiredService<IPredicateTemplateFactory>(), sp.GetRequiredService<IPredicateTemplateCache>(), sp.GetRequiredService<TDbContext>().Model));
    services.AddScoped<IPredicateTemplateFactory>(sp => new PredicateTemplateFactory(sp.GetRequiredService<TDbContext>().Model));
    services.AddSingleton<IPredicateTemplateCache>(PredicateTemplateCache.Create());

    services.AddScoped<IDefaultEntityFactory, DefaultEntityFactory>();
    services.AddScoped<IDefaultEntityFactoryFactory>(sp => new DefaultEntityFactoryFactory(sp.GetRequiredService<TDbContext>().Model));
    services.AddSingleton<IDefaultEntityFactoryCache>(DefaultEntityFactoryCache.Create());

    services.AddScoped<StateManager<TDbContext>>();
    services.AddScoped<IStateManager<TDbContext>>(sp => sp.GetRequiredService<StateManager<TDbContext>>());
    services.AddScoped<IUnitOfWorkManager>(sp => sp.GetRequiredService<StateManager<TDbContext>>());
    services.AddScoped(sp => sp.GetRequiredService<IUnitOfWorkManager>().Begin());
    
    services.AddScoped<IDataContext<TDbContext>, DataContext<TDbContext>>();
    services.AddScoped<IDataContext>(sp => sp.GetRequiredService<IDataContext<TDbContext>>());

    return services;
  }
}
