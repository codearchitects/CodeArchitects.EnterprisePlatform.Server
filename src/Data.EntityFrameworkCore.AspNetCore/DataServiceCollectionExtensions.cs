using CodeArchitects.Platform.Data.EntityFrameworkCore;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Materialization;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Query;
using CodeArchitects.Platform.Data.Tracking;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class DataServiceCollectionExtensions
{
  public static IServiceCollection AddData<TDbContext>(this IServiceCollection services)
    where TDbContext : DbContext
  {
    ArgumentNullException.ThrowIfNull(services);

    services.AddScoped<ITrackingContext, TrackingContext>();

    services.AddScoped<IPredicateProvider, PredicateProvider>();
    services.AddScoped<IPredicateTemplateFactory>(sp => new PredicateTemplateFactory(sp.GetRequiredService<TDbContext>().Model));
    services.AddSingleton<IPredicateTemplateCache>(PredicateTemplateCache.Create());

    services.AddScoped<IDefaultEntityFactory, DefaultEntityFactory>();
    services.AddScoped<IDefaultEntityFactoryFactory>(sp => new DefaultEntityFactoryFactory(sp.GetRequiredService<TDbContext>().Model));
    services.AddSingleton<IDefaultEntityFactoryCache>(DefaultEntityFactoryCache.Create());

    services.AddScoped<IStateManager<TDbContext>, StateManager<TDbContext>>();

    services.AddScoped<IEFCoreContext<TDbContext>, EFCoreContext<TDbContext>>();
    services.AddScoped<IEFCoreContext>(sp => sp.GetRequiredService<IEFCoreContext<TDbContext>>());

    return services;
  }
}
