using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

/// <summary>
/// Extension of <see cref="CaepOptionsBuilder"/> to enable the multitenancy feature.
/// </summary>
public static class MultitenancyCaepOptionsBuilderExtensions
{
  /// <summary>
  /// Enables the multitenancy feature within EFCore.
  /// </summary>
  /// <param name="builder">The options builder.</param>
  /// <param name="descriptor">The multitenancy descriptor.</param>
  /// <returns>The same <see cref="CaepOptionsBuilder"/> for further configuration.</returns>
  public static CaepOptionsBuilder UseMultitenancy(this CaepOptionsBuilder builder, IMultitenancyDescriptor descriptor)
  {
    ArgumentNullException.ThrowIfNull(builder);
    ArgumentNullException.ThrowIfNull(descriptor);

    return builder.UseMultitenancyCore(descriptor);
  }

  /// <summary>
  /// Enables the multitenancy feature within EFCore.
  /// </summary>
  /// <typeparam name="TDescriptor">The multitenancy descriptor type.</typeparam>
  /// <param name="builder">The options builder.</param>
  /// <returns>The same <see cref="CaepOptionsBuilder"/> for further configuration.</returns>
  public static CaepOptionsBuilder UseMultitenancy<TDescriptor>(this CaepOptionsBuilder builder)
    where TDescriptor : IMultitenancyDescriptor, new()
  {
    ArgumentNullException.ThrowIfNull(builder);
    
    return builder.UseMultitenancyCore(new TDescriptor());
  }

  /// <summary>
  /// Enables the multitenancy feature within EFCore using the default settings.
  /// </summary>
  /// <param name="builder">The options builder.</param>
  /// <param name="enableModificationFilters">Specifies whether the modification interceptors should be disable. Defaults to <c>false</c>.</param>
  /// <returns>The same <see cref="CaepOptionsBuilder"/> for further configuration.</returns>
  public static CaepOptionsBuilder UseMultitenancy(this CaepOptionsBuilder builder, bool enableModificationFilters = false)
  {
    ArgumentNullException.ThrowIfNull(builder);

    return builder.UseMultitenancyCore(new DefaultMultitenancyDescriptor<Guid>(enableModificationFilters));
  }

  private static CaepOptionsBuilder UseMultitenancyCore(this CaepOptionsBuilder builder, IMultitenancyDescriptor descriptor)
  {
    if (descriptor.MultitenancyContextType is null && descriptor.MultitenancyContextImplementationFactory is null)
      throw new ArgumentException($"The descriptor must specify a {nameof(IMultitenancyDescriptor)} type or factory.", nameof(descriptor));
    if (descriptor.TenantIdType is null)
      throw new ArgumentException("The descriptor must specify a tenant id type.", nameof(descriptor));
    if (!descriptor.TenantIdType.IsAssignableTo(typeof(IEquatable<>).MakeGenericType(descriptor.TenantIdType)))
      throw new ArgumentException($"The tenant id type must implement the generic '{typeof(IEquatable<>).Name}' interface.", nameof(descriptor));

    (builder as ICaepOptionsBuilderInfrastructure).AddOrUpdatePlugin(new MultitenancyCaepPlugin(descriptor));
    return builder;
  }
}
