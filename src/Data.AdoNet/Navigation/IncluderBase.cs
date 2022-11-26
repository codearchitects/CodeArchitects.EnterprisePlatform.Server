using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Data.Navigation;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal abstract class IncluderBase<TEntity> : IExpressionIncluder<TEntity>
  where TEntity : class
{
  protected abstract IncluderNode Node { get; }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression)
    where T : class
  {
    try
    {
      switch (includeExpression.Body)
      {
        case MemberExpression memberExpression:
          Node.AddLeaf(memberExpression);
          break;

        case NewExpression newExpression:
          if (!newExpression.Type.IsAnonymousType())
            throw new ArgumentException("Invalid include expression.", nameof(includeExpression));

          foreach (Expression argument in newExpression.Arguments)
          {
            if (argument is not MemberExpression memberExpression)
              throw new ArgumentException("Invalid include expression.", nameof(includeExpression));

            Node.AddLeaf(memberExpression);
          }
          break;

        default:
          throw new ArgumentException("Invalid include expression.", nameof(includeExpression));
      }

      return this;
    }
    catch (IncludeException ex)
    {
      throw WrapIncludeException(ex, nameof(includeExpression));
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
      throw WrapIncludeException(ex, nameof(includeExpression));
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
      throw WrapIncludeException(ex, nameof(includeExpression));
    }
  }

  private IExpressionIncluder<TEntity> Include<T>(MemberExpression memberExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    NavigationNode childNode = Node.AddNode(memberExpression);
    thenInclude(new ThenIncluder<T>(childNode));

    return this;
  }

  protected static ArgumentException WrapIncludeException(IncludeException ex, string parameterName)
    => new ArgumentException("Invalid include expression. See inner exception for details.", parameterName, ex);
}
