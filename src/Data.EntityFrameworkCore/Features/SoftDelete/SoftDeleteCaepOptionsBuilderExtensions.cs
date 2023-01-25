using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;

/// <summary>
/// Extension of <see cref="CaepOptionsBuilder"/> to enable the soft delete feature.
/// </summary>
public static class SoftDeleteCaepOptionsBuilderExtensions
{
  /// <summary>
  /// Enables the soft delete feature within EFCore.
  /// </summary>
  /// <param name="builder">The options builder.</param>
  /// <param name="descriptor">The soft delete descriptor.</param>
  /// <returns>The same <see cref="CaepOptionsBuilder"/> for further configuration.</returns>
  public static CaepOptionsBuilder UseSoftDelete(this CaepOptionsBuilder builder, ISoftDeleteDescriptor descriptor)
  {
    ArgumentNullException.ThrowIfNull(builder);
    ArgumentNullException.ThrowIfNull(descriptor);

    return builder.UseSoftDeleteCore(descriptor);
  }

  /// <summary>
  /// Enables the soft delete feature within EFCore.
  /// </summary>
  /// <typeparam name="TDescriptor">The soft delete descriptor type.</typeparam>
  /// <param name="builder">The options builder.</param>
  /// <returns>The same <see cref="CaepOptionsBuilder"/> for further configuration.</returns>
  public static CaepOptionsBuilder UseSoftDelete<TDescriptor>(this CaepOptionsBuilder builder)
    where TDescriptor : ISoftDeleteDescriptor, new()
  {
    ArgumentNullException.ThrowIfNull(builder);

    return builder.UseSoftDeleteCore(new TDescriptor());
  }

  /// <summary>
  /// Enables the multitenancy feature within EFCore using the default settings.
  /// </summary>
  /// <param name="builder">The options builder.</param>
  /// <returns>The same <see cref="CaepOptionsBuilder"/> for further configuration.</returns>
  public static CaepOptionsBuilder UseSoftDelete(this CaepOptionsBuilder builder)
  {
    ArgumentNullException.ThrowIfNull(builder);

    return builder.UseSoftDeleteCore(new DefaultSoftDeleteDescriptor());
  }

  private static CaepOptionsBuilder UseSoftDeleteCore(this CaepOptionsBuilder builder, ISoftDeleteDescriptor descriptor)
  {
    if (descriptor.SoftDeleteContextType is null && descriptor.SoftDeleteContextImplementationFactory is null)
      throw new ArgumentException($"The descriptor must specify a {nameof(ISoftDeleteDescriptor)} type or factory.", nameof(descriptor));

    (builder as ICaepOptionsBuilderInfrastructure).AddOrUpdatePlugin(new SoftDeleteCaepPlugin(descriptor));
    return builder;
  }
}
