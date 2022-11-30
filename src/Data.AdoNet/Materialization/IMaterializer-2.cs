using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal interface IMaterializer<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  TEntity? ReadEntity(IDataReader reader, ref int offset, IReadOnlyCollection<INavigation> navigations);
}
