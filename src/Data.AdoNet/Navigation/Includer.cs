using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Data.Navigation;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal abstract class Includer<TEntity> : IExpressionIncluder<TEntity>
  where TEntity : class
{
  protected abstract NavigationNode Node { get; }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression)
    where T : class
  {
    switch (includeExpression.Body)
    {
      case MemberExpression memberExpression:
        Node.AddLeaf(memberExpression);
        break;

      case NewExpression newExpression:
        if (!newExpression.Type.IsAnonymousType())
          throw new IncludeException(); // TODO: Message

        foreach (Expression argument in newExpression.Arguments)
        {
          if (argument is not MemberExpression memberExpression)
            throw new IncludeException(); // TODO: Message

          Node.AddLeaf(memberExpression);
        }
        break;

      default:
        throw new IncludeException(); // TODO: Message
    }

    return this;
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    if (includeExpression.Body is not MemberExpression memberExpression)
      throw new IncludeException(); // TODO: Message

    return Include(memberExpression, thenInclude);
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, IEnumerable<T>?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    if (includeExpression.Body is not MemberExpression memberExpression)
      throw new IncludeException(); // TODO: Message

    return Include(memberExpression, thenInclude);
  }

  private IExpressionIncluder<TEntity> Include<T>(MemberExpression memberExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    NavigationNode childNode = Node.AddNode(memberExpression);
    thenInclude(new ThenIncluder<T>(childNode));

    return this;
  }
}
