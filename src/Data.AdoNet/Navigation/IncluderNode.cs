using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.Navigation;
using System.Linq.Expressions;
using System.Text;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class IncluderNode<TEntity> : IExpressionIncluder<TEntity>, ISubNavigationSpec
  where TEntity : class
{
  private readonly INavigationModel _navigation;

  public IncluderNode(INavigationModel navigation)
  {
    _navigation = navigation;
  }

  public void WriteQueryPart(StringBuilder stringBuilder)
  {
    throw new NotImplementedException();
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression) where T : class
  {
    throw new NotImplementedException();
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude) where T : class
  {
    throw new NotImplementedException();
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, IEnumerable<T>?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude) where T : class
  {
    throw new NotImplementedException();
  }
}

internal class IncluderLeaf : ISubNavigationSpec
{
  private readonly INavigationModel _navigation;

  public IncluderLeaf(INavigationModel navigation)
  {
    _navigation = navigation;
  }

  public void WriteQueryPart(StringBuilder stringBuilder)
  {
    throw new NotImplementedException();
  }
}
