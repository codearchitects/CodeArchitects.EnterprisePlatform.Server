namespace CodeArchitects.Platform.Data.AdoNet.Model;

public interface IPrimaryKeyModel<TKey> : IPrimaryKeyModel
  where TKey : IEquatable<TKey>
{
  new IAccessor<TKey> Accessor { get; }

  object GetKeyComponent(TKey key, int index);
}
