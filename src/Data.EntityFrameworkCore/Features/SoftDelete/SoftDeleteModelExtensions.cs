using CodeArchitects.Platform.Common.Exceptions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;

public static class SoftDeleteModelExtensions
{
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
