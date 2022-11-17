using CodeArchitects.Platform.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Query;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

[Experimental]
public interface IInterceptedEntityQueryProvider : IAsyncQueryProvider
{
  void EnableInterceptor(Type interceptorType);
  void DisableInterceptor(Type interceptorType);
}
