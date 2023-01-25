using CodeArchitects.Platform.Data.EntityFrameworkCore.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

internal class ConventionSetPlugin : IConventionSetPlugin
{
  private readonly Type _tenantIdType;

  public ConventionSetPlugin(Type tenantIdType)
  {
    _tenantIdType = tenantIdType;
  }

  public ConventionSet ModifyConventions(ConventionSet conventionSet)
  {
    Convention convention = new(_tenantIdType);

    conventionSet.ModelInitializedConventions.Add(convention);
    conventionSet.ModelFinalizedConventions.Add(convention);
    return conventionSet;
  }

  private class Convention : IModelInitializedConvention, IModelFinalizedConvention
  {
    private readonly Type _tenantIdType;

    public Convention(Type tenantIdType)
    {
      _tenantIdType = tenantIdType;
    }

    public IModel ProcessModelFinalized(IModel model)
    {
      foreach (IEntityType entityType in model.GetEntityTypes())
      {
        if (entityType.TryGetTenantIdPropertyName(out string? propertyName))
        {
          IProperty property = entityType.GetProperty(propertyName);
          if (property.IsPrimaryKey())
            continue;

          entityType.HasTenantIdProperty(property);
          entityType.HasMultitenancyPredicateTemplate(MakePredicateTemplate(entityType, property));
          entityType.HasMultitenancyColumnMappings(property.GetTableColumnMappings());
        }
      }

      return model;
    }

    public void ProcessModelInitialized(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
    {
      modelBuilder.HasTenantIdType(_tenantIdType);
    }

    private static LambdaExpression MakePredicateTemplate(IEntityType entityType, IProperty property)
    {
      ParameterExpression entity = Expression.Parameter(entityType.ClrType, nameof(entity));

      return Expression.Lambda(
        body: Expression.Equal(
          left: ExpressionHelpers.MakePropertyAccess(entity, property),
          right: Expression.Variable(property.ClrType)),
        entity);
    }
  }
}
