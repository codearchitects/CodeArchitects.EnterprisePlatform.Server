using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal interface IMaterializer<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  // TODO: Optimize when only one element is expected
  void Setup(ICollection<TEntity> entities);

  void ReadRow(DbDataReader reader);

  void Reset();
}
