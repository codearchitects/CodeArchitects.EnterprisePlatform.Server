using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

public interface IQueryRootExpressionInterceptor
{
  bool ShouldApply { get; }
  Expression Apply(Expression expression, IEntityType entity);
}
