using CodeArchitects.Platform.Common.CodeAnalysis;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;

/// <summary>
/// Soft delete extension methods for <see cref="IQueryable{T}"/>.
/// </summary>
public static class SoftDeleteQueryableExtensions
{
  internal static readonly MethodInfo s_asNoSoftDeleteMethodInfo = typeof(SoftDeleteQueryableExtensions).GetRequiredMethod(
    name: nameof(AsNoSoftDelete),
    bindingAttr: BindingFlags.Static | BindingFlags.Public,
    types: new[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)) });

  internal static readonly MethodInfo s_asSoftDeleteMethodInfo = typeof(SoftDeleteQueryableExtensions).GetRequiredMethod(
    name: nameof(AsSoftDelete),
    bindingAttr: BindingFlags.Static | BindingFlags.Public,
    types: new[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)) });

  /// <summary>
  /// Disables the soft delete filter for the query.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <param name="source">The queryable.</param>
  /// <returns>An <see cref="IQueryable{T}"/> that will ignore the soft delete filter.</returns>
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

  /// <summary>
  /// Enables the soft delete filter for the query.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <param name="source">The queryable.</param>
  /// <returns>An <see cref="IQueryable{T}"/> that will use the soft delete filter.</returns>
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
