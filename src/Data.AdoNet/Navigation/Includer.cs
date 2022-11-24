using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.Navigation;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class Includer<TEntity> : IIncluder<TEntity>
  where TEntity : class
{
  private readonly IEntityModel _entity;

  public Includer(IEntityModel entity)
  {
    _entity = entity;
  }

  public string BuildSelectQuery()
  {
    throw new NotImplementedException();
  }

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

    if (!_entity.TryGetNavigation(navigation, out INavigationModel navigationModel))
      throw new ArgumentException($"Navigation '{navigation}' does not exist on entity '{_entity.Name}'.", nameof(navigation));

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

    if (!_entity.TryGetNavigation(navigation, out INavigationModel navigationModel))
      throw new ArgumentException($"Navigation '{navigation}' does not exist on entity '{_entity.Name}'.", nameof(navigation));

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

    if (!_entity.TryGetNavigation(navigation, out INavigationModel navigationModel))
      throw new ArgumentException($"Navigation '{navigation}' does not exist on entity '{_entity.Name}'.", nameof(navigation));

    // _navigationSpecs.Add(navigationModel);

    return this;
  }
}
