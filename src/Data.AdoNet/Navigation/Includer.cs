using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.Navigation;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class Includer<TEntity> : IIncluder<TEntity>, INavigationRoot
  where TEntity : class
{
  private readonly Dictionary<int, INavigation> _navigations;

  public Includer(IEntityModel entity)
  {
    Entity = entity;
    _navigations = new();
  }

  public IEntityModel Entity { get; }

  public IReadOnlyCollection<INavigation> Navigations => _navigations.Values;

  // entity => entity.Child;
  // entity => new { entity.Child1, entity.Child2 }
  // entity => entity.Child.GrandChild;
  // entity => new { entity.Child1, entity.Child2.GrandChild }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression)
    where T : class
  {
    if (includeExpression.Body is not MemberExpression member)
      throw new ArgumentException("", nameof(includeExpression)); // TODO: Support other expressions

    string navigationName = member.Member switch
    {
      PropertyInfo property => property.Name,
      FieldInfo field => field.Name,
      _ => throw new ArgumentException($"The specified member is not a property or a field.", nameof(includeExpression))
    };

    if (!Entity.TryGetNavigation(navigationName, out INavigationModel model))
      throw new ArgumentException($"Navigation '{navigationName}' does not exist on entity '{Entity.Name}'.", nameof(includeExpression));

    if (!_navigations.ContainsKey(model.Id))
    {
      _navigations.Add(model.Id, new IncluderLeaf(model));
    }

    return this;
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    if (includeExpression.Body is not MemberExpression member)
      throw new ArgumentException("", nameof(includeExpression)); // TODO: Support other expressions

    string navigationName = member.Member switch
    {
      PropertyInfo property => property.Name,
      FieldInfo field => field.Name,
      _ => throw new ArgumentException($"The specified member is not a property or a field.", nameof(includeExpression))
    };

    if (!Entity.TryGetNavigation(navigationName, out INavigationModel model))
      throw new ArgumentException($"Navigation '{navigationName}' does not exist on entity '{Entity.Name}'.", nameof(includeExpression));

    if (!_navigations.TryGetValue(model.Id, out INavigation? navigation) && navigation is IncluderNode<T> node)
    {
      thenInclude(node);
      return this;
    }

    node = new IncluderNode<T>(model);
    thenInclude(node);
    _navigations[model.Id] = node;

    return this;
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, IEnumerable<T>?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    return this;
  }

  public IStringIncluder<TEntity> Include(string navigation)
  {

    if (!Entity.TryGetNavigation(navigation, out INavigationModel model))
      throw new ArgumentException($"Navigation '{navigation}' does not exist on entity '{Entity.Name}'.", nameof(navigation));

    _navigations.Add(model.Id, new IncluderLeaf(model));

    return this;
  }
}
