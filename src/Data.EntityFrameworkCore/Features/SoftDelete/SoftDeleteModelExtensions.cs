using CodeArchitects.Platform.Common.Exceptions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;

/// <summary>
/// Extension methods to configure entity types for soft delete.
/// </summary>
public static class SoftDeleteModelExtensions
{
  /// <summary>
  /// Specifies that an entity uses the soft delete policy.
  /// </summary>
  /// <typeparam name="TEntity">The entity type being configured.</typeparam>
  /// <typeparam name="TProperty">The soft delete property type.</typeparam>
  /// <param name="builder">The entity type builder.</param>
  /// <param name="propertyExpression">A lambda expression representing the soft delete property.</param>
  /// <returns>The same <see cref="EntityTypeBuilder{TEntity}"/> for further configuration.</returns>
  public static EntityTypeBuilder<TEntity> IsSoftDelete<TEntity, TProperty>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, TProperty>> propertyExpression)
    where TEntity : class
  {
    ArgumentNullException.ThrowIfNull(builder);
    ArgumentNullException.ThrowIfNull(propertyExpression);

    IMutableProperty property = builder.Property(propertyExpression).Metadata;

    if (!IsSoftDeleteType(property.ClrType))
      throw new ArgumentException("The soft delete flag must be a boolean property.", nameof(propertyExpression));

    return builder.HasSoftDeletePropertyName(property.Name);
  }

  /// <summary>
  /// Specifies that an entity uses the soft delete policy.
  /// </summary>
  /// <typeparam name="TEntity">The entity type being configured.</typeparam>
  /// <typeparam name="TProperty">The soft delete property type.</typeparam>
  /// <param name="builder">The entity type builder.</param>
  /// <param name="propertyName">The name of the soft delete property.</param>
  /// <returns>The same <see cref="EntityTypeBuilder{TEntity}"/> for further configuration.</returns>
  public static EntityTypeBuilder<TEntity> IsSoftDelete<TEntity, TProperty>(this EntityTypeBuilder<TEntity> builder, string propertyName)
    where TEntity : class
  {
    ArgumentNullException.ThrowIfNull(builder);
    ArgumentNullException.ThrowIfNull(propertyName);

    if (!IsSoftDeleteType(typeof(TProperty)))
      throw new TypeArgumentException("The soft delete flag must be a boolean or a nullable property.", nameof(TProperty));

    IMutableProperty property = builder.Property(typeof(TProperty), propertyName).Metadata;

    return builder.HasSoftDeletePropertyName(property.Name);
  }

  /// <summary>
  /// Specifies that an entity uses the soft delete policy.
  /// </summary>
  /// <typeparam name="TEntity">The entity type being configured.</typeparam>
  /// <param name="builder">The entity type builder.</param>
  /// <param name="propertyName">The name of the soft delete property.</param>
  /// <returns>The same <see cref="EntityTypeBuilder{TEntity}"/> for further configuration.</returns>
  public static EntityTypeBuilder<TEntity> IsSoftDelete<TEntity>(this EntityTypeBuilder<TEntity> builder, string propertyName)
    where TEntity : class
  {
    ArgumentNullException.ThrowIfNull(builder);
    ArgumentNullException.ThrowIfNull(propertyName);

    IMutableProperty property = builder.Property(typeof(bool), propertyName).Metadata;

    return builder.HasSoftDeletePropertyName(property.Name);
  }

  private static bool IsSoftDeleteType(Type type) // TODO: Support nullable types (needs value factory)
  {
    return type == typeof(bool);
    // return type == typeof(bool) || !type.IsValueType || Nullable.GetUnderlyingType(type) is not null;
  }
}
