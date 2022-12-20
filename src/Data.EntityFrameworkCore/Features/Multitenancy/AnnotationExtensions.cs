using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

internal static class AnnotationExtensions
{
  public static IConventionModelBuilder HasTenantIdType(this IConventionModelBuilder builder, Type tenantIdType)
  {
    builder.HasAnnotation(MultitenancyAnnotationNames.TenantIdType, tenantIdType);
    return builder;
  }

  public static Type GetTenantIdType(this IReadOnlyModel model)
  {
    if (!model.TryGetAnnotationValue(MultitenancyAnnotationNames.TenantIdType, out Type? type))
      throw new InvalidOperationException($"Multitenancy was not configured. Add a call to '{nameof(MultitenancyCaepOptionsBuilderExtensions.UseMultitenancy)}' to your {nameof(CaepExtension)} configuration.");

    return type;
  }


  public static EntityTypeBuilder<TEntity> HasTenantIdPropertyName<TEntity>(this EntityTypeBuilder<TEntity> builder, string propertyName)
    where TEntity : class
  {
    return builder.HasAnnotation(MultitenancyAnnotationNames.PropertyName, propertyName);
  }

  public static bool TryGetTenantIdPropertyName(this IEntityType entityType, [NotNullWhen(true)] out string? propertyName)
  {
    return entityType.TryGetAnnotationValue(MultitenancyAnnotationNames.PropertyName, out propertyName);
  }


  public static IAnnotation HasTenantIdProperty(this IEntityType entityType, IProperty property)
  {
    return entityType.AddRuntimeAnnotation(MultitenancyAnnotationNames.Property, property);
  }

  public static bool TryGetTenantIdProperty(this IEntityType entityType, [NotNullWhen(true)] out IProperty? property)
  {
    return entityType.TryGetRuntimeAnnotationValue(MultitenancyAnnotationNames.Property, out property);
  }


  public static IAnnotation HasMultitenancyPredicateTemplate(this IEntityType entityType, LambdaExpression template)
  {
    return entityType.AddRuntimeAnnotation(MultitenancyAnnotationNames.PredicateTemplate, template);
  }

  public static bool TryGetMultitenancyPredicateTemplate(this IEntityType entityType, [NotNullWhen(true)] out LambdaExpression? template)
  {
    return entityType.TryGetRuntimeAnnotationValue(MultitenancyAnnotationNames.PredicateTemplate, out template);
  }


  public static IAnnotation HasMultitenancyColumnMappings(this IEntityType entityType, IEnumerable<IColumnMapping> mappings)
  {
    return entityType.AddRuntimeAnnotation(MultitenancyAnnotationNames.ColumnMappings, mappings);
  }

  public static bool TryGetMultitenancyColumnMappings(this IEntityType entityType, [NotNullWhen(true)] out IEnumerable<IColumnMapping>? mappings)
  {
    return entityType.TryGetRuntimeAnnotationValue(MultitenancyAnnotationNames.ColumnMappings, out mappings);
  }
}
