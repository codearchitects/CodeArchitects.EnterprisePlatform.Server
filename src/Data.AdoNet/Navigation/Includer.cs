using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.Navigation;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class Includer<TEntity> : IIncluder<TEntity>, INavigationRoot
  where TEntity : class
{
  private readonly List<INavigation> _navigations;

  public Includer(IEntityModel entity)
  {
    Entity = entity;
    _navigations = new();
  }

  public IEntityModel Entity { get; }

  public IReadOnlyList<INavigation> Navigations => _navigations;

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression)
    where T : class
  {
    if (includeExpression.Body is not MemberExpression member)
      throw new ArgumentException("", nameof(includeExpression)); // TODO: Support other expressions

    string navigation = member.Member switch
    {
      PropertyInfo property => property.Name,
      FieldInfo field => field.Name,
      _ => throw new ArgumentException($"The specified member is not a property or a field.", nameof(includeExpression))
    };

    if (!Entity.TryGetNavigation(navigation, out INavigationModel navigationModel))
      throw new ArgumentException($"Navigation '{navigation}' does not exist on entity '{Entity.Name}'.", nameof(navigation));

    // _navigationSpecs.Add(navigationModel);

    return this;
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, T?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    if (includeExpression.Body is not MemberExpression member)
      throw new ArgumentException("", nameof(includeExpression)); // TODO: Support other expressions

    string navigation = member.Member switch
    {
      PropertyInfo property => property.Name,
      FieldInfo field => field.Name,
      _ => throw new ArgumentException($"The specified member is not a property or a field.", nameof(includeExpression))
    };

    if (!Entity.TryGetNavigation(navigation, out INavigationModel navigationModel))
      throw new ArgumentException($"Navigation '{navigation}' does not exist on entity '{Entity.Name}'.", nameof(navigation));

    // _navigationSpecs.Add(navigationModel);


    return this;
  }

  public IExpressionIncluder<TEntity> Include<T>(Expression<Func<TEntity, IEnumerable<T>?>> includeExpression, Action<IExpressionIncluder<T>> thenInclude)
    where T : class
  {
    return this;
  }

  public IStringIncluder<TEntity> Include(string navigation)
  {
    // TODO: string[] paths = navigation.Split('.');

    if (!Entity.TryGetNavigation(navigation, out INavigationModel model))
      throw new ArgumentException($"Navigation '{navigation}' does not exist on entity '{Entity.Name}'.", nameof(navigation));

    _navigations.Add(new IncluderLeaf(model));

    return this;
  }
}
