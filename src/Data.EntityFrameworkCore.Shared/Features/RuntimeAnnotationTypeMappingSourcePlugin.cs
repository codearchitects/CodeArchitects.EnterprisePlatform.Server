using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features;

internal class RuntimeAnnotationTypeMappingSourcePlugin : IRelationalTypeMappingSourcePlugin
{
  public RelationalTypeMapping? FindMapping(in RelationalTypeMappingInfo mappingInfo)
  {
    if (mappingInfo.ClrType is { } clrType && clrType.IsGenericType && clrType.GetGenericTypeDefinition() == typeof(RuntimeAnnotationWrapper<>))
    {
      return new RuntimeAnnotationRelationalTypeMapping(clrType.GetGenericArguments()[0]);
    }

    return null;
  }

  private class RuntimeAnnotationRelationalTypeMapping : RelationalTypeMapping
  {
    public RuntimeAnnotationRelationalTypeMapping(Type clrType)
      : base("", clrType)
    {
    }

    public override Expression GenerateCodeLiteral(object value)
    {
      return Expression.Constant("<runtime annotation>");
    }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters)
    {
      return new RuntimeAnnotationRelationalTypeMapping(parameters.CoreParameters.ClrType);
    }
  }
}
