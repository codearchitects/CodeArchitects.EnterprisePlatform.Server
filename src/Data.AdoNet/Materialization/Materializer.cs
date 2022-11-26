using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal abstract class Materializer<TEntity, TKey> : IMaterializer<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  private readonly HashSet<TKey> _keys;
  private ICollection<TEntity>? _entities;

  public Materializer()
  {
    _keys = new();
  }

  public void ReadRow(DbDataReader reader)
  {
    if (_entities is null)
      throw new InvalidOperationException("Materializer was not set up.");

    TKey key = ReadKey(reader);
    if (_keys.Contains(key))
      return;

    _entities.Add(ReadRest(key, reader));
    _keys.Add(key);
  }

  public void Reset()
  {
    _keys.Clear();
    _entities = null;
  }

  public void Setup(ICollection<TEntity> entities)
  {
    _entities = entities;
  }

  protected abstract TKey ReadKey(DbDataReader reader);

  protected abstract TEntity ReadRest(TKey key, DbDataReader reader);
}
