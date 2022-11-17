using CodeArchitects.Platform.CodeAnalysis;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

public static class MultitenancyQueryableExtensions
{
  internal static readonly MethodInfo s_asNoMultitenancyMethodInfo;
  internal static readonly MethodInfo s_asMultitenancyMethodInfo;

  static MultitenancyQueryableExtensions()
  {
    s_asNoMultitenancyMethodInfo = typeof(MultitenancyQueryableExtensions).GetRequiredMethod(
      name: nameof(AsNoMultitenancy),
      bindingAttr: BindingFlags.Static | BindingFlags.Public,
      types: new[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)) });

    s_asMultitenancyMethodInfo = typeof(MultitenancyQueryableExtensions).GetRequiredMethod(
      name: nameof(AsMultitenancy),
      bindingAttr: BindingFlags.Static | BindingFlags.Public,
      types: new[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)) });
  }

  [Experimental]
  public static IQueryable<TEntity> AsNoMultitenancy<TEntity>(this IQueryable<TEntity> source)
    where TEntity : class
  {
    if (source.Provider is IInterceptedEntityQueryProvider provider)
    {
      provider.DisableInterceptor(typeof(QueryRootExpressionInterceptor));
    }

    return source;
  }

  [Experimental]
  public static IQueryable<TEntity> AsMultitenancy<TEntity>(this IQueryable<TEntity> source)
    where TEntity : class
  {
    if (source.Provider is IInterceptedEntityQueryProvider provider)
    {
      provider.EnableInterceptor(typeof(QueryRootExpressionInterceptor));
    }

    return source;
  }
}
