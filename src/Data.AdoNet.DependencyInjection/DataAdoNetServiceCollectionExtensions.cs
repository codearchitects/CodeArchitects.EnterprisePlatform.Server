using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Data;
using CodeArchitects.Platform.Data.AdoNet;
using CodeArchitects.Platform.Data.AdoNet.Command;
using CodeArchitects.Platform.Data.AdoNet.DependencyInjection;
using CodeArchitects.Platform.Data.AdoNet.Executor;
using CodeArchitects.Platform.Data.AdoNet.Features.Concurrency;
using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using CodeArchitects.Platform.Data.AdoNet.Materialization;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using CodeArchitects.Platform.Data.Tracking;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;
using IDataContext = CodeArchitects.Platform.Data.AdoNet.IDataContext;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Methods for adding the ADO.NET data context to the application services.
/// </summary>
public static class DataAdoNetServiceCollectionExtensions
{
  /// <summary>
  /// Injects the services needed to support the ADO.NET data context.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <param name="configurationAction">An action that specifies the ADO.NET configuration.</param>
  /// <returns>The same <see cref="IServiceCollection"/> instance.</returns>
  public static IServiceCollection AddData(this IServiceCollection services, Func<IAdoNetConfigurationBuilder, IAdoNetConfigurationBuilderWithProvider> configurationAction)
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));
    if (configurationAction is null)
      throw new ArgumentNullException(nameof(configurationAction));

    AdoNetConfigurationBuilder configurationBuilder = new();
    configurationAction(configurationBuilder);

    DatabaseProvider provider = configurationBuilder.Provider;
    Type providerType = provider.GetType();
    services.AddSingleton(providerType, provider);
    services.AddSingleton(typeof(DatabaseProvider), sp => sp.GetRequiredService(providerType));

    // Seed
    if (configurationBuilder.SeedType is { } seedType)
    {
      services.AddScoped(typeof(DataSeed), seedType);
    }

    // Command
    services.AddSingleton<ISqlTextCache, SqlTextCache>();
    services.AddSingleton(provider.CreateSyntaxProvider());
    services.AddSingleton<ISqlTextBuilder, SqlTextBuilder>();
    services.TryAddSingleton<IMemoryCache>(_ => new MemoryCache(new MemoryCacheOptions { SizeLimit = 10240 }));

    Type commandBuilderServiceType = provider.MakeGenericType(typeof(ICommandBuilder<>));
    services.AddScoped(commandBuilderServiceType, sp => provider.CreateCommandBuilder(sp.GetRequiredService<ISqlTextBuilder>(), sp.GetRequiredService<IConcurrencyContext>()));

    // Materializer
    services.TryAddSingleton<IIdentityCollectionFactory>(IdentityCollectionFactory.Create());
    services.TryAddSingleton<IRowReaderProvider>(RowReaderProvider.Create());
    services.TryAddScoped<IMaterializer, Materializer>();

    // Tracking
    services.TryAddScoped<ITrackingContext, TrackingContext>();

    // Interceptors
    Type commandInterceptorServiceType = provider.MakeGenericType(typeof(ICommandInterceptor<>));
    Type commandInterceptorAggregatorServiceType = provider.MakeGenericType(typeof(ICommandInterceptorAggregator<>));
    Type commandInterceptorAggregatorImplementationType;

    if (configurationBuilder.CommandInterceptorTypes is { Count: > 0 } commandInterceptorImplementationTypes)
    {
      commandInterceptorAggregatorImplementationType = provider.MakeGenericType(typeof(CommandInterceptorAggregator<>));

      foreach (Type commandInterceptorImplementationType in commandInterceptorImplementationTypes)
      {
        services.AddScoped(commandInterceptorServiceType, commandInterceptorImplementationType);
      }
    }
    else
    {
      commandInterceptorAggregatorImplementationType = provider.MakeGenericType(typeof(NullCommandInterceptorAggregator<>));
    }

    services.AddScoped(commandInterceptorAggregatorServiceType, commandInterceptorAggregatorImplementationType);

    // Executor
    Type executorServiceType = provider.MakeGenericType(typeof(IExecutor<>));
    Type executorImplementationType = provider.MakeGenericType(typeof(Executor<>));

    services.AddScoped(executorServiceType, executorImplementationType);

    // State manager
    Type connectionFactoryServiceType = provider.MakeGenericType(typeof(IConnectionFactory<>));
    Type stateManagerServiceType = provider.MakeGenericType(typeof(IStateManager<>));
    Type stateManagerImplementationType = provider.MakeGenericType(typeof(StateManager<>));

    if (provider.ConnectionFactoryType is { } connectionFactoryType)
    {
      services.AddScoped(connectionFactoryServiceType, connectionFactoryType);
    }
    else if (provider.DelegateConnectionFactory is { } connectionFactory)
    {
      services.AddSingleton(connectionFactoryServiceType, connectionFactory);
    }
    else
    {
      throw new InvalidOperationException("No connection factory was specified.");
    }

    services.AddScoped(stateManagerImplementationType);
    services.AddScoped(stateManagerServiceType, sp => sp.GetRequiredService(stateManagerImplementationType));
    services.AddScoped(typeof(IUnitOfWorkManager), sp => sp.GetRequiredService(stateManagerImplementationType));
    services.AddScoped(sp => sp.GetRequiredService<IUnitOfWorkManager>().Begin());

    // Data model
    services.TryAddSingleton(typeof(ModelConfiguration), configurationBuilder.ModelConfigurationType);
    services.TryAddSingleton(sp => sp.GetRequiredService<ModelConfiguration>().CreateDataModel());

    // Navigation
    services.TryAddSingleton<INavigationTreeFactory, NavigationTreeFactory>();

    // Concurrency
    services.TryAddScoped<IConcurrencyContext, ConcurrencyContext>();
    services.Add(configurationBuilder.ConcurrencyTokenProviderDescriptor);

    // Data context
    Type dataContextType = provider.MakeGenericType(typeof(IDataContext<>));

    services.AddScoped(dataContextType, provider.DataContextType);
    services.AddScoped(sp => (IDataContext)sp.GetRequiredService(dataContextType));

    // Utils
    services.TryAddTransient<Synchronizer>();

    return services;
  }
}
