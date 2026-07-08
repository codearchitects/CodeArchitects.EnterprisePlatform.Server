using CodeArchitects.Platform.Common.CodeAnalysis;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

/// <summary>
/// Multitenancy extension methods for <see cref="IQueryable{T}"/>.
/// </summary>
public static class MultitenancyQueryableExtensions
{
  internal static readonly MethodInfo s_asNoMultitenancyMethodInfo = typeof(MultitenancyQueryableExtensions).GetRequiredMethod(
    name: nameof(AsNoMultitenancy),
    bindingAttr: BindingFlags.Static | BindingFlags.Public,
    types: new[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)) });
  
  internal static readonly MethodInfo s_asMultitenancyMethodInfo = typeof(MultitenancyQueryableExtensions).GetRequiredMethod(
    name: nameof(AsMultitenancy),
    bindingAttr: BindingFlags.Static | BindingFlags.Public,
    types: new[] { typeof(IQueryable<>).MakeGenericType(Type.MakeGenericMethodParameter(0)) });


  /// <summary>
  /// Disables the multitenancy filter for the query.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <param name="source">The queryable.</param>
  /// <returns>An <see cref="IQueryable{T}"/> that will ignore the multitenancy filter.</returns>
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

  /// <summary>
  /// Enables the multitenancy filter for the query.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <param name="source">The queryable.</param>
  /// <returns>An <see cref="IQueryable{T}"/> that will use the multitenancy filter.</returns>
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
