using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal interface IMaterializerFactory
{
  IMaterializer<TEntity, TKey> CreateMaterializer<TEntity, TKey>(IEntityModel entity, IEnumerable<INavigation> navigations)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}