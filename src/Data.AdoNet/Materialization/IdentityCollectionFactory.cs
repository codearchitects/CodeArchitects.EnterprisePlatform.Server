using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class IdentityCollectionFactory : IIdentityCollectionFactory
{
  private static readonly MethodInfo s_createIdentityCollectionMethod = typeof(IdentityCollectionFactory).GetRequiredMethod(
    name: nameof(CreateIdentityCollection),
    bindingAttr: BindingFlags.Static | BindingFlags.NonPublic);

  public delegate IIdentityCollection Factory(CollectionKind collectionKind);

  private readonly ConcurrentDictionary<IEntityModel, Factory> _factories;

  public IdentityCollectionFactory(ConcurrentDictionary<IEntityModel, Factory> factories)
  {
    _factories = factories;
  }

  public IIdentityCollection CreateCollection(INavigationModel navigation)
  {
    Debug.Assert(navigation.CollectionKind is not CollectionKind.None);

    return _factories.GetOrAdd(navigation.To, CreateFactory).Invoke(navigation.CollectionKind);
  }

  private static IIdentityCollection CreateIdentityCollection<TEntity, TKey>(IEntityModel<TEntity, TKey> entity, CollectionKind collectionKind)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    EntityEqualityComparer<TEntity, TKey> comparer = EntityEqualityComparer<TEntity, TKey>.GetDefault(entity);

    return collectionKind switch
    {
      CollectionKind.List    => new IdentityList<TEntity>(comparer),
      CollectionKind.HashSet => new IdentityHashSet<TEntity>(comparer),
      _                      => throw new ArgumentException($"Invalid collection kind: '{collectionKind}'.", nameof(collectionKind))
    };
  }

  private Factory CreateFactory(IEntityModel entity)
  {
    MethodInfo createIdentityCollectionMethod = s_createIdentityCollectionMethod.MakeGenericMethod(entity.Type, entity.PrimaryKey.Type);
    Type entityModelType = typeof(IEntityModel<,>).MakeGenericType(entity.Type, entity.PrimaryKey.Type);

    ParameterExpression collectionKindParam = Expression.Parameter(typeof(CollectionKind), "collectionKind");

    Expression<Factory> factoryExpression = Expression.Lambda<Factory>(
      body: Expression.Call(
        method: createIdentityCollectionMethod,
        arg0: Expression.Constant(entity, entityModelType),
        arg1: collectionKindParam),
      parameters: collectionKindParam);

    return factoryExpression.Compile();
  }

  public static IdentityCollectionFactory Create()
  {
    return new(new ConcurrentDictionary<IEntityModel, Factory>());
  }
}
