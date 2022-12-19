using CodeArchitects.Platform.Data.AdoNet;
using CodeArchitects.Platform.Data.AdoNet.Command;
using CodeArchitects.Platform.Data.AdoNet.DependencyInjection;
using CodeArchitects.Platform.Data.AdoNet.Executor;
using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using CodeArchitects.Platform.Data.AdoNet.Materialization;
using CodeArchitects.Platform.Data.Tracking;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class DataAdoNetServiceCollectionExtensions
{
  public static IServiceCollection AddData(this IServiceCollection services, Func<IAdoNetConfigurationBuilder, IAdoNetConfigurationBuilderWithProvider> configurationAction)
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));
    if (configurationAction is null)
      throw new ArgumentNullException(nameof(configurationAction));

    if (services.Any(service => service.ServiceType == typeof(IDataContext)))
      throw new InvalidOperationException($"'{nameof(AddData)}' was already called. Having multiple database providers at the same time is not supported.");

    AdoNetConfigurationBuilder configurationBuilder = new();
    configurationAction(configurationBuilder);

    DatabaseProvider provider = configurationBuilder.Provider;

    // Command
    Type commandBuilderServiceType = provider.MakeGenericType(typeof(ICommandBuilder<>));

    services.AddSingleton(typeof(ISyntaxProvider), provider.SyntaxProviderType);
    services.AddSingleton<ISqlTextCache>(SqlTextCache.Create());
    services.AddSingleton<ISqlTextBuilder, SqlTextBuilder>();
    services.AddSingleton(commandBuilderServiceType, provider.CommandBuilderType);

    // Materializer
    services.AddSingleton<IIdentityCollectionFactory>(IdentityCollectionFactory.Create());
    services.AddSingleton<IRowReaderProvider>(RowReaderProvider.Create());
    services.AddScoped<IMaterializer, Materializer>();

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

    services.AddScoped(stateManagerServiceType, stateManagerImplementationType);

    // Data model
    services.AddSingleton(typeof(ModelConfiguration), configurationBuilder.ModelConfigurationType);
    services.AddSingleton(sp => sp.GetRequiredService<ModelConfiguration>().CreateDataModel());

    // Data context
    Type dataContextType = provider.MakeGenericType(typeof(IDataContext<>));

    services.AddScoped(dataContextType, provider.DataContextType);
    services.AddScoped(sp => (IDataContext)sp.GetRequiredService(dataContextType));

    return services;
  }
}
