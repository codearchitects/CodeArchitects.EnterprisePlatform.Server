using CodeArchitects.Platform.Data;
using CodeArchitects.Platform.Data.AdoNet;
using CodeArchitects.Platform.Data.AdoNet.DependencyInjection;
using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using CodeArchitects.Platform.Data.Features.Concurrency;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// An object used to configure the ADO.NET context and services.
/// </summary>
public interface IAdoNetConfigurationBuilder
{
  /// <summary>
  /// Specifies the data provider to use.
  /// </summary>
  /// <typeparam name="TProvider">The database provider type.</typeparam>
  /// <param name="configureAction">An action to configure the provider.</param>
  /// <returns>A <see cref="IAdoNetConfigurationBuilderWithProvider"/> for further configuration.</returns>
  IAdoNetConfigurationBuilderWithProvider UseProvider<TProvider>(Action<TProvider> configureAction)
    where TProvider : DatabaseProvider, new();
}

/// <summary>
/// An object used to configure the ADO.NET context and services.
/// </summary>
public interface IAdoNetConfigurationBuilderWithProvider
{
  /// <summary>
  /// Specifies the model configuration to use to build the persistence model.
  /// </summary>
  /// <param name="modelConfigurationType">The model configuration type.</param>
  /// <returns>An <see cref="IAdoNetConfigurationBuilderWithProvider"/> for further configuration.</returns>
  IAdoNetConfigurationBuilderWithProvider UseModel(Type modelConfigurationType);

  /// <summary>
  /// Adds a command interceptor to the services.
  /// </summary>
  /// <param name="interceptorType">The interceptor type.</param>
  /// <returns>An <see cref="IAdoNetConfigurationBuilderWithProvider"/> for further configuration.</returns>
  IAdoNetConfigurationBuilderWithProvider AddCommandInterceptor(Type interceptorType);

  /// <summary>
  /// Specifies the seed type to use for seeding the database.
  /// </summary>
  /// <param name="seedType">The seed type. It must extend <see cref="DataSeed"/>.</param>
  /// <returns>An <see cref="IAdoNetConfigurationBuilderWithProvider"/> for further configuration.</returns>
  IAdoNetConfigurationBuilderWithProvider UseSeed(Type seedType);

  /// <summary>
  /// Specifies the provider to use for generating concurrency tokens.
  /// </summary>
  /// <param name="tokenProviderType">The concurrency token provider implementation type. It must implement <see cref="IConcurrencyTokenProvider"/>.</param>
  /// <param name="tokenProviderLifetime">The lifetime of the <see cref="IConcurrencyTokenProvider"/> service.</param>
  /// <returns>An <see cref="IAdoNetConfigurationBuilderWithProvider"/> for further configuration.</returns>
  IAdoNetConfigurationBuilderWithProvider UseConcurrencyTokenProvider(Type tokenProviderType, ServiceLifetime tokenProviderLifetime = ServiceLifetime.Singleton);

  /// <summary>
  /// Specifies the provider to use for generating concurrency tokens.
  /// </summary>
  /// <param name="tokenProvider">The concurrency token provider instance that will generate concurrency tokens.</param>
  /// <returns>An <see cref="IAdoNetConfigurationBuilderWithProvider"/> for further configuration.</returns>
  IAdoNetConfigurationBuilderWithProvider UseConcurrencyTokenProvider(IConcurrencyTokenProvider tokenProvider);

  /// <summary>
  /// Scans the specifies assembly and looks for a <see cref="ModelConfiguration"/> type to use and registers any found <see cref="ICommandInterceptor{TCommand}"/> implementation.
  /// </summary>
  /// <param name="assembly">The assembly to scan.</param>
  /// <param name="serviceTypes">Specifies which services to look for.</param>
  /// <returns>An <see cref="IAdoNetConfigurationBuilderWithProvider"/> for further configuration.</returns>
  IAdoNetConfigurationBuilderWithProvider ScanAssemblyForServices(Assembly assembly, AdoNetServiceTypes serviceTypes = AdoNetServiceTypes.All);
}