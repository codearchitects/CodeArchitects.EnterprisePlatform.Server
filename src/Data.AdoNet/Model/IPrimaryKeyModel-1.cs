namespace CodeArchitects.Platform.Data.AdoNet.Model;

public interface IPrimaryKeyModel<TKey> : IPrimaryKeyModel
  where TKey : IEquatable<TKey>
{
  object GetKeyComponent(TKey key, int index);
}
