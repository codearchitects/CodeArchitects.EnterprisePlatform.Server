using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

/// <summary>
/// Extension of the <see cref="IServiceCollection"/> interface that exposes methods to register special service types.
/// </summary>
public interface IPluginServiceCollection : IServiceCollection
{
  /// <summary>
  /// Adds a modification interceptor service.
  /// </summary>
  /// <param name="implementationType">The interceptor implementation type.</param>
  /// <returns>The same <see cref="IPluginServiceCollection"/> for further configuration.</returns>
  IPluginServiceCollection AddModificationInterceptor(Type implementationType);

  /// <summary>
  /// Adds a modification interceptor service.
  /// </summary>
  /// <param name="implementationFactory">The interceptor implementation factory.</param>
  /// <returns>The same <see cref="IPluginServiceCollection"/> for further configuration.</returns>
  IPluginServiceCollection AddModificationInterceptor(Func<IServiceProvider, object> implementationFactory);

  /// <summary>
  /// Adds a query root expression interceptor service.
  /// </summary>
  /// <param name="implementationType">The interceptor implementation type.</param>
  /// <returns>The same <see cref="IPluginServiceCollection"/> for further configuration.</returns>
  IPluginServiceCollection AddQueryRootExpressionInterceptor(Type implementationType);

  /// <summary>
  /// Adds a query root expression interceptor service.
  /// </summary>
  /// <param name="implementationFactory">The interceptor implementation factory.</param>
  /// <returns>The same <see cref="IPluginServiceCollection"/> for further configuration.</returns>
  IPluginServiceCollection AddQueryRootExpressionInterceptor(Func<IServiceProvider, object> implementationFactory);
}

/// <summary>
/// Extension methods for <see cref="IPluginServiceCollection"/>.
/// </summary>
public static class PluginServiceCollectionExtensions
{
  /// <summary>
  /// Adds a modification interceptor service.
  /// </summary>
  /// <typeparam name="TImplementation">The interceptor implementation type.</typeparam>
  /// <param name="services">The service collection.</param>
  /// <returns>The same <see cref="IPluginServiceCollection"/> for further configuration.</returns>
  public static IPluginServiceCollection AddModificationInterceptor<TImplementation>(this IPluginServiceCollection services)
    where TImplementation : IModificationInterceptor
  {
    return services.AddModificationInterceptor(typeof(TImplementation));
  }

  /// <summary>
  /// Adds a query root expression interceptor service.
  /// </summary>
  /// <typeparam name="TImplementation">The interceptor implementation type.</typeparam>
  /// <param name="services">The service collection.</param>
  /// <returns>The same <see cref="IPluginServiceCollection"/> for further configuration.</returns>
  public static IPluginServiceCollection AddQueryRootExpressionInterceptor<TImplementation>(this IPluginServiceCollection services)
    where TImplementation : IQueryRootExpressionInterceptor
  {
    return services.AddQueryRootExpressionInterceptor(typeof(TImplementation));
  }
}