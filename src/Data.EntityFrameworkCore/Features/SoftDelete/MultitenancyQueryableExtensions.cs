using CodeArchitects.Platform.CodeAnalysis;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;

public static class SoftDeleteQueryableExtensions
{
  internal static readonly MethodInfo s_asNoSoftDeleteMethodInfo;
  internal static readonly MethodInfo s_asSoftDeleteMethodInfo;

  static SoftDeleteQueryableExtensions()
  {
    s_asNoSoftDeleteMethodInfo = typeof(SoftDeleteQueryableExtensions).GetRequiredMethod(
      name: nameof(AsNoSoftDelete),
      bindingAttr: BindingFlags.Static | BindingFlags.Public,
      types: new[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)) });

    s_asSoftDeleteMethodInfo = typeof(SoftDeleteQueryableExtensions).GetRequiredMethod(
      name: nameof(AsSoftDelete),
      bindingAttr: BindingFlags.Static | BindingFlags.Public,
      types: new[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)) });
  }

  [Experimental]
  public static IQueryable<TEntity> AsNoSoftDelete<TEntity>(this IQueryable<TEntity> source)
    where TEntity : class
  {
    if (source.Provider is IInterceptedEntityQueryProvider provider)
    {
      provider.DisableInterceptor(typeof(QueryRootExpressionInterceptor));
    }

    return source;
  }

  [Experimental]
  public static IQueryable<TEntity> AsSoftDelete<TEntity>(this IQueryable<TEntity> source)
    where TEntity : class
  {
    if (source.Provider is IInterceptedEntityQueryProvider provider)
    {
      provider.EnableInterceptor(typeof(QueryRootExpressionInterceptor));
    }

    return source;
  }
}
