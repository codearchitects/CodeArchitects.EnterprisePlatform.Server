using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;

internal static class AnnotationExtensions
{
  public static EntityTypeBuilder<TEntity> HasSoftDeletePropertyName<TEntity>(this EntityTypeBuilder<TEntity> builder, string propertyName)
    where TEntity : class
  {
    return builder.HasAnnotation(SoftDeleteAnnotationNames.PropertyName, propertyName);
  }

  public static bool TryGetSoftDeletePropertyName(this IEntityType entityType, [NotNullWhen(true)] out string? propertyName)
  {
    return entityType.TryGetAnnotationValue(SoftDeleteAnnotationNames.PropertyName, out propertyName);
  }


  public static IAnnotation HasSoftDeleteProperty(this IEntityType entityType, IProperty property)
  {
    return entityType.AddRuntimeAnnotation(SoftDeleteAnnotationNames.Property, property);
  }

  public static bool TryGetSoftDeleteProperty(this IEntityType entityType, [NotNullWhen(true)] out IProperty? property)
  {
    return entityType.TryGetRuntimeAnnotationValue(SoftDeleteAnnotationNames.Property, out property);
  }


  public static IAnnotation HasSoftDeletePredicate(this IEntityType entityType, LambdaExpression template)
  {
    return entityType.AddRuntimeAnnotation(SoftDeleteAnnotationNames.Predicate, template);
  }

  public static bool TryGetSoftDeletePredicate(this IEntityType entityType, [NotNullWhen(true)] out LambdaExpression? template)
  {
    return entityType.TryGetRuntimeAnnotationValue(SoftDeleteAnnotationNames.Predicate, out template);
  }


  public static IAnnotation HasSoftDeleteColumnMappings(this IEntityType entityType, IEnumerable<IColumnMapping> mappings)
  {
    return entityType.AddRuntimeAnnotation(SoftDeleteAnnotationNames.ColumnMappings, mappings);
  }

  public static bool TryGetSoftDeleteColumnMappings(this IEntityType entityType, [NotNullWhen(true)] out IEnumerable<IColumnMapping>? mappings)
  {
    return entityType.TryGetRuntimeAnnotationValue(SoftDeleteAnnotationNames.ColumnMappings, out mappings);
  }
}
