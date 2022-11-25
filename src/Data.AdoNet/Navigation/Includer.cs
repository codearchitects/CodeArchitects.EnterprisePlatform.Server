using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.Navigation;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class Includer<TEntity> : IncluderBase, IIncluder<TEntity>, INavigationRoot
  where TEntity : class
{
  public Includer(IEntityModel entity)
  {
    Target = entity;
  }

  public IReadOnlyCollection<INavigation> Navigations => Children;

  public IEntityModel Entity => Target;

  protected override IEntityModel Target { get; }

  // entity => entity.Child;
  // entity => new { entity.Child1, entity.Child2 }
  // entity => entity.Child.GrandChild;
  // entity => new { entity.Child1, entity.Child2.GrandChild }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression)
    where T : class
  {
    throw new NotImplementedException();
    //if (includeExpression.Body is not MemberExpression member)
    //  throw new ArgumentException("", nameof(includeExpression)); // TODO: Support other expressions

    //string navigationName = member.Member switch
    //{
    //  PropertyInfo property => property.Name,
    //  FieldInfo field => field.Name,
    //  _ => throw new ArgumentException($"The specified member is not a property or a field.", nameof(includeExpression))
    //};

    //if (!Entity.TryGetNavigation(navigationName, out INavigationModel model))
    //  throw new ArgumentException($"Navigation '{navigationName}' does not exist on entity '{Entity.Name}'.", nameof(includeExpression));

    //if (!_navigations.ContainsKey(model.Id))
    //{
    //  _navigations.Add(model.Id, new IncluderLeaf(model));
    //}

    //return this;
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    throw new NotImplementedException();
    //if (includeExpression.Body is not MemberExpression member)
    //  throw new ArgumentException("", nameof(includeExpression)); // TODO: Support other expressions

    //string navigationName = member.Member switch
    //{
    //  PropertyInfo property => property.Name,
    //  FieldInfo field => field.Name,
    //  _ => throw new ArgumentException($"The specified member is not a property or a field.", nameof(includeExpression))
    //};

    //if (!Entity.TryGetNavigation(navigationName, out INavigationModel model))
    //  throw new ArgumentException($"Navigation '{navigationName}' does not exist on entity '{Entity.Name}'.", nameof(includeExpression));

    //if (!_navigations.TryGetValue(model.Id, out INavigation? navigation) && navigation is IncluderNode<T> node)
    //{
    //  thenInclude(node);
    //  return this;
    //}

    //node = new IncluderNode<T>(model);
    //thenInclude(node);
    //_navigations[model.Id] = node;

    //return this;
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, IEnumerable<T>?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    return this;
  }

  public IStringIncluder<TEntity> Include(string navigation)
  {
    if (!TryInclude(navigation.AsSpan()))
      throw new ArgumentException($"'{navigation}' is an invalid navigation path for entity '{Target.Type.Name}'.", nameof(navigation));

    return this;
  }
}
