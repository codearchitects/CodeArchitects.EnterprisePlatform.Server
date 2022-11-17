namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

public static class MultitenancyAnnotationNames
{
  public const string Prefix = "CA:Multitenancy:";

  public const string TenantIdType = Prefix + nameof(TenantIdType);

  public const string PredicateTemplate = Prefix + nameof(PredicateTemplate);

  public const string PropertyName = Prefix + nameof(PropertyName);

  public const string Property = Prefix + nameof(Property);

  public const string ColumnMappings = Prefix + nameof(ColumnMappings);
}
