using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

/// <summary>
/// Extension methods to configure entity types for multitenancy.
/// </summary>
public static class MultitenancyModelExtensions
{
  /// <summary>
  /// Specifies that an entity is multi-tenant.
  /// </summary>
  /// <typeparam name="TEntity">The entity type being configured.</typeparam>
  /// <typeparam name="TProperty">The multitenancy property type.</typeparam>
  /// <param name="builder">The entity type builder.</param>
  /// <param name="propertyExpression">A lambda expression representing the tenant id property.</param>
  /// <returns>The same <see cref="EntityTypeBuilder{TEntity}"/> for further configuration.</returns>
  public static EntityTypeBuilder<TEntity> IsMultiTenant<TEntity, TProperty>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, TProperty>> propertyExpression)
    where TEntity : class
  {
    ArgumentNullException.ThrowIfNull(builder);
    ArgumentNullException.ThrowIfNull(propertyExpression);

    Type tenantIdType = builder.Metadata.Model.GetTenantIdType();
    if (typeof(TProperty) != tenantIdType)
      throw new ArgumentException($"The tenant id property must be of the same type that was registered at startup ('{tenantIdType.Name}').", nameof(propertyExpression));

    IMutableProperty property = builder.Property(propertyExpression).Metadata;

    return builder.HasTenantIdPropertyName(property.Name);
  }

  /// <summary>
  /// Specifies that an entity is multi-tenant.
  /// </summary>
  /// <typeparam name="TEntity">The entity type being configured.</typeparam>
  /// <typeparam name="TProperty">The multitenancy property type.</typeparam>
  /// <param name="builder">The entity type builder.</param>
  /// <param name="propertyName">The name of the tenant id property.</param>
  /// <returns>The same <see cref="EntityTypeBuilder{TEntity}"/> for further configuration.</returns>
  public static EntityTypeBuilder<TEntity> IsMultiTenant<TEntity>(this EntityTypeBuilder<TEntity> builder, string propertyName)
    where TEntity : class
  {
    ArgumentNullException.ThrowIfNull(builder);
    ArgumentNullException.ThrowIfNull(propertyName);
    
    Type tenantIdType = builder.Metadata.Model.GetTenantIdType();
    IMutableProperty property = builder.Property(tenantIdType, propertyName).Metadata;

    return builder.HasTenantIdPropertyName(property.Name);
  }
}
