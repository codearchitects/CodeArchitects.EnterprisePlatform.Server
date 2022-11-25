using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.Navigation;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class IncluderNode<TEntity> : IncluderNode, IExpressionIncluder<TEntity>
  where TEntity : class
{
  public IncluderNode(INavigationModel model)
    : base(model)
  {
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression)
    where T : class
  {
    throw new NotImplementedException();
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    throw new NotImplementedException();
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, IEnumerable<T>?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    throw new NotImplementedException();
  }
}
