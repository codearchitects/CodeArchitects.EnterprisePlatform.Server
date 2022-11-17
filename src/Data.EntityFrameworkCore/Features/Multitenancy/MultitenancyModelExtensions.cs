using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

public static class MultitenancyModelExtensions
{
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
