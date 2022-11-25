using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.Navigation;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class Includer<TEntity> : IIncluder<TEntity>, INavigationRoot
  where TEntity : class
{
  private readonly IncluderNode _node;

  private Includer(IncluderNode node)
  {
    _node = node;
  }

  public Includer(IEntityModel entity)
  {
    _node = new IncluderRoot(entity);
  }

  public IEntityModel Entity => _node.Target;

  public IReadOnlyCollection<INavigation> Navigations => _node.Children;

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression)
    where T : class
  {
    try
    {
      switch (includeExpression.Body)
      {
        case MemberExpression memberExpression:
          _node.AddLeaf(memberExpression);
          break;

        case NewExpression newExpression:
          if (!newExpression.Type.IsAnonymousType())
            throw new ArgumentException("Invalid include expression.", nameof(includeExpression));

          foreach (Expression argument in newExpression.Arguments)
          {
            _node.AddLeaf((MemberExpression)argument);
          }
          break;

        default:
          throw new ArgumentException("Invalid include expression.", nameof(includeExpression));
      }

      return this;
    }
    catch (IncludeException ex)
    {
      throw GetWrappedException(nameof(includeExpression), ex);
    }
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    if (includeExpression.Body is not MemberExpression memberExpression)
      throw new ArgumentException("Invalid include expression.", nameof(includeExpression));

    try
    {
      return Include(memberExpression, thenInclude);
    }
    catch (IncludeException ex)
    {
      throw GetWrappedException(nameof(includeExpression), ex);
    }
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, IEnumerable<T>?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    if (includeExpression.Body is not MemberExpression memberExpression)
      throw new ArgumentException("Invalid include expression.", nameof(includeExpression));

    try
    {
      return Include(memberExpression, thenInclude);
    }
    catch (IncludeException ex)
    {
      throw GetWrappedException(nameof(includeExpression), ex);
    }
  }

  public IStringIncluder<TEntity> Include(string navigation)
  {
    try
    {
      _node.AddLeaf(navigation.AsSpan());

      return this;
    }
    catch (IncludeException ex)
    {
      throw GetWrappedException(nameof(navigation), ex);
    }
  }

  private IExpressionIncluder<TEntity> Include<T>(MemberExpression memberExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    NavigationNode childNode = _node.AddNode(memberExpression);
    thenInclude(new Includer<T>(childNode));

    return this;
  }

  private static ArgumentException GetWrappedException(string parameterName, IncludeException ex)
    => new ArgumentException("Invalid include expression. See inner exception for details", parameterName, ex);
}
