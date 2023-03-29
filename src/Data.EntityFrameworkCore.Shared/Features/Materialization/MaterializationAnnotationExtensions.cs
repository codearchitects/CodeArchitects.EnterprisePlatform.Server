using CodeArchitects.Platform.Data.EntityFrameworkCore.Helpers;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Materialization;

using TEntityType =
#if NET6_0_OR_GREATER
  IReadOnlyEntityType
#else
  IEntityType
#endif
  ;

/// <summary>
/// Extension methods for configuring materialization.
/// </summary>
public static class MaterializationAnnotationExtensions
{
  /// <summary>
  /// Specifies a factory function that can be used to create an entity instance, given its primary key.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="builder">The entity builder.</param>
  /// <param name="factory">The factory.</param>
  /// <returns>The same <see cref="EntityTypeBuilder{TEntity}"/> for further configuration.</returns>
  public static EntityTypeBuilder<TEntity> HasDefaultFactory<TEntity, TKey>(this EntityTypeBuilder<TEntity> builder, Func<TKey, TEntity> factory)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (builder is null)
      throw new ArgumentException(nameof(builder));
    if (factory is null)
      throw new ArgumentException(nameof(factory));

    if (EFCoreEnvironment.IsDesignTime)
      return builder;

    builder.HasAnnotation(MaterializationAnnotationNames.DefaultFactory, factory);
    return builder;
  }

  /// <summary>
  /// Tries to get the default factory for a given entity type.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <typeparam name="TKey">The entity's primary key type.</typeparam>
  /// <param name="entityType">The entity type.</param>
  /// <param name="factory">The default factory, if specified, <c>null</c> otherwise.</param>
  /// <returns><c>true</c> if a default factory is found, <c>false</c> otherwise.</returns>
  public static bool TryGetDefaultFactory<TEntity, TKey>(this TEntityType entityType, [NotNullWhen(true)] out Func<TKey, TEntity>? factory)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return entityType.TryGetAnnotationValue(MaterializationAnnotationNames.DefaultFactory, out factory);
  }
}
