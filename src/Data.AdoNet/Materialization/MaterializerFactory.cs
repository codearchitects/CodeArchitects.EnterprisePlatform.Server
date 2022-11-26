using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class MaterializerFactory : IMaterializerFactory
{
  private delegate object Factory(IMaterializerFactory factory, IEnumerable<INavigation> navigations);

  private readonly IReadOnlyDictionary<Type, Factory> _factories;

  private MaterializerFactory(IReadOnlyDictionary<Type, Factory> factories)
  {
    _factories = factories;
  }

  public IMaterializer<TEntity, TKey> CreateMaterializer<TEntity, TKey>(IEntityModel entity, IEnumerable<INavigation> navigations)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    Factory factory = _factories[entity.Type];
    return (IMaterializer<TEntity, TKey>)factory(this, navigations);
  }
}
