using CodeArchitects.Platform.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Query;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

/// <summary>
/// Extension of <see cref="IAsyncQueryProvider"/> that enables interception.
/// </summary>
[Experimental]
public interface IInterceptedEntityQueryProvider : IAsyncQueryProvider
{
  /// <summary>
  /// Enables the specified interceptor type, regardless of the <see cref="IQueryRootExpressionInterceptor.ShouldApply"/> property.
  /// </summary>
  /// <param name="interceptorType">The interceptor type.</param>
  void EnableInterceptor(Type interceptorType);

  /// <summary>
  /// Disables the specified interceptor type, regardless of the <see cref="IQueryRootExpressionInterceptor.ShouldApply"/> property.
  /// </summary>
  /// <param name="interceptorType">The interceptor type.</param>
  void DisableInterceptor(Type interceptorType);
}
