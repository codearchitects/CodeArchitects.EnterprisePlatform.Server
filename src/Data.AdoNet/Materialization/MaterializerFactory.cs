using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class MaterializerFactory : IMaterializerFactory
{
  private readonly IReadOnlyDictionary<Type, Func<IMaterializerHub, object>> _factories;

  public MaterializerFactory(IReadOnlyDictionary<Type, Func<IMaterializerHub, object>> factories)
  {
    _factories = factories;
  }

  public IMaterializer<TEntity, TKey> CreateMaterializer<TEntity, TKey>(IMaterializerHub hub)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    return (IMaterializer<TEntity, TKey>)_factories[typeof(TEntity)](hub);
  }

  public static MaterializerFactory Create(IEnumerable<EntityEmitResult> infos)
  {
    Dictionary<Type, Func<IMaterializerHub, object>> factories = new Dictionary<Type, Func<IMaterializerHub, object>>();

    foreach (EntityEmitResult info in infos)
    {
      ConstructorInfo constructor = info.MaterializerType.GetRequiredConstructor(
        bindingAttr: BindingFlags.Instance | BindingFlags.Public,
        types: new[] { typeof(IMaterializerHub) });

      ParameterExpression hubParam = Expression.Parameter(typeof(IMaterializerHub), "hub");

      Expression<Func<IMaterializerHub, object>> factoryExpression = Expression.Lambda<Func<IMaterializerHub, object>>(
        body: Expression.New(
          constructor: constructor,
          arguments: new[] { hubParam }),
        parameters: new[] { hubParam });

      factories.Add(info.EntityType, factoryExpression.Compile());
    }

    return new MaterializerFactory(factories);
  }
}
