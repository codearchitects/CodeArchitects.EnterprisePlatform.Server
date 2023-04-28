using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

/// <summary>
/// Extension methods for <see cref="IPluginServiceCollection"/>.
/// </summary>
public static class PluginServiceCollectionExtensions
{
  /// <summary>
  /// Adds a modification interceptor service.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <param name="implementationType">The interceptor implementation type.</param>
  /// <returns>The same <see cref="IPluginServiceCollection"/> for further configuration.</returns>
  public static IPluginServiceCollection AddModificationInterceptor(this IPluginServiceCollection services, Type implementationType)
  {
    AddModificationCommandFactory(services);
    services.AddScoped(typeof(IModificationInterceptor), implementationType);

    return services;
  }

  /// <summary>
  /// Adds a modification interceptor service.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <param name="implementationFactory">The interceptor implementation factory.</param>
  /// <returns>The same <see cref="IPluginServiceCollection"/> for further configuration.</returns>
  public static IPluginServiceCollection AddModificationInterceptor(this IPluginServiceCollection services, Func<IServiceProvider, object> implementationFactory)
  {
    AddModificationCommandFactory(services);
    services.AddScoped(typeof(IModificationInterceptor), implementationFactory);

    return services;
  }

  /// <summary>
  /// Adds a query root expression interceptor service.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <param name="implementationType">The interceptor implementation type.</param>
  /// <returns>The same <see cref="IPluginServiceCollection"/> for further configuration.</returns>
  public static IPluginServiceCollection AddQueryRootExpressionInterceptor(this IPluginServiceCollection services, Type implementationType)
  {
    AddEntityQueryProvider(services);
    services.AddScoped(typeof(IQueryRootExpressionInterceptor), implementationType);

    return services;
  }

  /// <summary>
  /// Adds a query root expression interceptor service.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <param name="implementationFactory">The interceptor implementation factory.</param>
  /// <returns>The same <see cref="IPluginServiceCollection"/> for further configuration.</returns>
  public static IPluginServiceCollection AddQueryRootExpressionInterceptor(this IPluginServiceCollection services, Func<IServiceProvider, object> implementationFactory)
  {
    AddEntityQueryProvider(services);
    services.AddScoped(typeof(IQueryRootExpressionInterceptor), implementationFactory);

    return services;
  }

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

  private static void AddModificationCommandFactory(IPluginServiceCollection services)
  {
    services.RemoveAll<IModificationCommandFactory>();
    services.AddSingleton<IModificationCommandFactory, InterceptedModificationCommandFactory>();
  }

  private static void AddEntityQueryProvider(IPluginServiceCollection services)
  {
    services.RemoveAll<IAsyncQueryProvider>();
    services.AddScoped<IAsyncQueryProvider, InterceptedEntityQueryProvider>();
    services.TryAddScoped<IExpressionRewriter, ExpressionRewriter>();
  }
}