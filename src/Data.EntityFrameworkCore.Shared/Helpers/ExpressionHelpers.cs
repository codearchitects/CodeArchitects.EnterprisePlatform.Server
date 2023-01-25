using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Helpers;

internal class ExpressionHelpers
{
  private static readonly MethodInfo s_efPropertyMethod;
  private static readonly MethodInfo s_whereMethod;

  static ExpressionHelpers()
  {
    s_efPropertyMethod = typeof(EF).GetRequiredMethod(
      name: nameof(EF.Property),
      bindingAttr: BindingFlags.Static | BindingFlags.Public,
      types: new[] { typeof(object), typeof(string) });

    s_whereMethod = typeof(Queryable).GetRequiredMethod(
      name: nameof(Queryable.Where),
      bindingAttr: BindingFlags.Static | BindingFlags.Public,
      types: new[]
      {
        typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)),
        typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(Type.MakeGenericMethodParameter(0), typeof(bool)))
      });
  }

#if NET6_0_OR_GREATER
  public static Expression MakePropertyAccess(Expression entity, IReadOnlyProperty property)
#else
  public static Expression MakePropertyAccess(Expression entity, IProperty property)
#endif
  {
    if (property.PropertyInfo is { } propertyInfo)
      return Expression.Property(entity, propertyInfo);

    if (property.FieldInfo is { } fieldInfo)
      return Expression.Field(entity, fieldInfo);

    return MakeShadowPropertyAccess(entity, property.Name, property.ClrType);
  }

  public static Expression MakeShadowPropertyAccess(Expression entity, string propertyName, Type propertyType)
  {
    return Expression.Call(
      instance: null,
      method: s_efPropertyMethod.MakeGenericMethod(propertyType),
      arg0: entity,
      arg1: Expression.Constant(propertyName));
  }

  public static Expression GetWhereExpression(Expression queryable, Expression predicate, Type entityType)
  {
    return Expression.Call(
      instance: null,
      method: s_whereMethod.MakeGenericMethod(entityType),
      arg0: queryable,
      arg1: Expression.Quote(predicate));
  }
}
