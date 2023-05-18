using CodeArchitects.Platform.Data.EntityFrameworkCore.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;

internal class ConventionSetPlugin : IConventionSetPlugin
{
  public ConventionSet ModifyConventions(ConventionSet conventionSet)
  {
    conventionSet.ModelFinalizedConventions.Add(new Convention());
    return conventionSet;
  }

  private sealed class Convention : IModelFinalizedConvention
  {
    public IModel ProcessModelFinalized(IModel model)
    {
      foreach (IEntityType entityType in model.GetEntityTypes())
      {
        if (entityType.TryGetSoftDeletePropertyName(out string? propertyName))
        {
          IProperty property = entityType.GetProperty(propertyName);
          if (property.IsPrimaryKey())
            continue;

          entityType.HasSoftDeleteProperty(property);
          entityType.HasSoftDeletePredicate(MakePredicateTemplate(entityType, property));
          entityType.HasSoftDeleteColumnMappings(property.GetTableColumnMappings());
        }
      }

      return model;
    }

    private static LambdaExpression MakePredicateTemplate(IEntityType entityType, IProperty property)
    {
      ParameterExpression entity = Expression.Parameter(entityType.ClrType, nameof(entity));
      Expression propertyAccess = ExpressionHelpers.MakePropertyAccess(entity, property);

      Type propertyType = property.ClrType;

      Expression body = propertyType == typeof(bool)
        ? FromBooleanProperty(propertyAccess)
        : FromNullableProperty(propertyAccess, propertyType);

      return Expression.Lambda(body, entity);

      static Expression FromBooleanProperty(Expression propertyAccess)
      {
        return Expression.Not(propertyAccess);
      }

      static Expression FromNullableProperty(Expression propertyAccess, Type propertyType)
      {
        return Expression.NotEqual(
          left: propertyAccess,
          right: Expression.Default(propertyType));
      }
    }
  }
}
