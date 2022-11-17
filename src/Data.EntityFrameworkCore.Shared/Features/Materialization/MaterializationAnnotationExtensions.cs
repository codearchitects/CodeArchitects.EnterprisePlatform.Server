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

public static class MaterializationAnnotationExtensions
{
  public static EntityTypeBuilder<TEntity> HasDefaultFactory<TEntity, TKey>(this EntityTypeBuilder<TEntity> builder, Func<TKey, TEntity> factory)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (builder is null)
      throw new ArgumentException(nameof(builder));
    if (factory is null)
      throw new ArgumentException(nameof(factory));

    builder.HasAnnotation(MaterializationAnnotationNames.DefaultFactory, RuntimeAnnotationWrapper.Create(factory));
    return builder;
  }

  internal static bool TryGetDefaultFactory<TEntity, TKey>(this TEntityType entityType, [NotNullWhen(true)] out Func<TKey, TEntity>? factory)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return entityType.TryGetRuntimeAnnotationValue(MaterializationAnnotationNames.DefaultFactory, out factory);
  }
}
