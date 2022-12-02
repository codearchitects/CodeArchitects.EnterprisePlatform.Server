namespace CodeArchitects.Platform.Data.AdoNet.Model;

public interface IPrimaryKeyModel<TKey> : IPrimaryKeyModel
  where TKey : IEquatable<TKey>
{
  Getter<TKey> Getter { get; }

  Setter<TKey> Setter { get; }

  object GetKeyComponent(TKey key, int index);
}
