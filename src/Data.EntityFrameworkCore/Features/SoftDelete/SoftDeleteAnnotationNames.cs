namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;

public static class SoftDeleteAnnotationNames
{
  public const string Prefix = "CA:SoftDelete:";

  public const string Predicate = Prefix + nameof(Predicate);

  public const string PropertyName = Prefix + nameof(PropertyName);

  public const string Property = Prefix + nameof(Property);

  public const string ColumnMappings = Prefix + nameof(ColumnMappings);
}
