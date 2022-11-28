using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal interface IMaterializer<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  void ReadEntity(DbDataReader reader, ref int offset, IReadOnlyCollection<INavigation> navigations, IIdentityCollection<TEntity> collection);

  void ReadEntity(DbDataReader reader, ref int offset, IReadOnlyCollection<INavigation> navigations, ref TEntity? reference);
}
