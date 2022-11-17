using CodeArchitects.Platform.Data.Navigation;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class Includer<TEntity> : IIncluder<TEntity>
  where TEntity : class
{
  private readonly List<string> _paths;

  public Includer()
  {
    _paths = new List<string>();
  }

  public IReadOnlyCollection<string> Paths => _paths;

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

  public IStringIncluder<TEntity> Include(string navigation)
  {
    _paths.Add(navigation);
    return this;
  }
}
