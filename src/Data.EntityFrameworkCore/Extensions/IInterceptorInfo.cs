namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

internal interface IInterceptorInfo
{
  bool IsEnabled(IQueryRootExpressionInterceptor interceptor);
}
