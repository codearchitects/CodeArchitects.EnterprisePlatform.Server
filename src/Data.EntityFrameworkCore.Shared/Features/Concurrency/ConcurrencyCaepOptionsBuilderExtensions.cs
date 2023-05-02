using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using CodeArchitects.Platform.Data.Features.Concurrency;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Concurrency;

/// <summary>
/// Extension of <see cref="CaepOptionsBuilder"/> to enable the optimistic concurrency feature.
/// </summary>
public static class ConcurrencyCaepOptionsBuilderExtensions
{
  /// <summary>
  /// Enables the optimistic concurrency feature within EFCore.
  /// </summary>
  /// <param name="builder">The options builder.</param>
  /// <returns>The same <see cref="CaepOptionsBuilder"/> for further configuration.</returns>
  public static CaepOptionsBuilder UseOptimisticConcurrency(this CaepOptionsBuilder builder)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    return builder.UseOptimisticConcurrencyCore(new ServiceDescriptor(typeof(IConcurrencyTokenProvider), typeof(ConcurrencyTokenProvider), ServiceLifetime.Singleton));
  }

  /// <summary>
  /// Enables the optimistic concurrency feature within EFCore.
  /// </summary>
  /// <param name="builder">The options builder.</param>
  /// <param name="tokenProviderType">The concurrency token provider implementation type. It must implement <see cref="IConcurrencyTokenProvider"/>.</param>
  /// <param name="tokenProviderLifetime">The lifetime of the <see cref="IConcurrencyTokenProvider"/> service.</param>
  /// <returns>The same <see cref="CaepOptionsBuilder"/> for further configuration.</returns>
  public static CaepOptionsBuilder UseOptimisticConcurrency(this CaepOptionsBuilder builder, Type tokenProviderType, ServiceLifetime tokenProviderLifetime = ServiceLifetime.Singleton)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));
    if (tokenProviderType is null)
      throw new ArgumentNullException(nameof(tokenProviderType));
    if (typeof(IConcurrencyTokenProvider).IsAssignableFrom(tokenProviderType) || !tokenProviderType.IsClass || tokenProviderType.IsAbstract)
      throw new ArgumentException($"'{nameof(tokenProviderType)}' should be a concrete class assignable to '{nameof(IConcurrencyTokenProvider)}'.", nameof(tokenProviderType));

    return builder.UseOptimisticConcurrencyCore(new ServiceDescriptor(typeof(IConcurrencyTokenProvider), tokenProviderType, tokenProviderLifetime));
  }

  /// <summary>
  /// Enables the optimistic concurrency feature within EFCore.
  /// </summary>
  /// <typeparam name="TTokenProvider">The concurrency token provider implementation type.</typeparam>
  /// <param name="builder">The options builder.</param>
  /// <param name="tokenProviderLifetime">The lifetime of the <see cref="IConcurrencyTokenProvider"/> service.</param>
  /// <returns>The same <see cref="CaepOptionsBuilder"/> for further configuration.</returns>
  public static CaepOptionsBuilder UseOptimisticConcurrency<TTokenProvider>(this CaepOptionsBuilder builder, ServiceLifetime tokenProviderLifetime = ServiceLifetime.Singleton)
    where TTokenProvider : class, IConcurrencyTokenProvider
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    Type tokenProviderType = typeof(TTokenProvider);
    if (tokenProviderType.IsAbstract)
      throw new TypeArgumentException($"'{nameof(TTokenProvider)}' should be a concrete class assignable to '{nameof(IConcurrencyTokenProvider)}'.", nameof(tokenProviderType));

    return builder.UseOptimisticConcurrencyCore(new ServiceDescriptor(typeof(IConcurrencyTokenProvider), tokenProviderType, tokenProviderLifetime));
  }

  /// <summary>
  /// Enables the optimistic concurrency feature within EFCore.
  /// </summary>
  /// <param name="builder">The options builder.</param>
  /// <param name="tokenProvider">The concurrency token provider instance that will generate concurrency tokens.</param>
  /// <returns>The same <see cref="CaepOptionsBuilder"/> for further configuration.</returns>
  public static CaepOptionsBuilder UseOptimisticConcurrency(this CaepOptionsBuilder builder, IConcurrencyTokenProvider tokenProvider)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));
    if (tokenProvider is null)
      throw new ArgumentNullException(nameof(tokenProvider));

    return builder.UseOptimisticConcurrencyCore(new ServiceDescriptor(typeof(IConcurrencyTokenProvider), tokenProvider));
  }

  private static CaepOptionsBuilder UseOptimisticConcurrencyCore(this CaepOptionsBuilder builder, ServiceDescriptor tokenProviderDescriptor)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    (builder as ICaepOptionsBuilderInfrastructure).AddOrUpdatePlugin(new ConcurrencyCaepPlugin(tokenProviderDescriptor));
    return builder;
  }
}
