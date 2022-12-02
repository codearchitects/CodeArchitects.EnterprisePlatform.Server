using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IPrimaryKeyModel<TKey> : IPrimaryKeyModel
  where TKey : IEquatable<TKey>
{
  Getter<TKey> Getter { get; }

  Setter<TKey> Setter { get; }

  object GetKeyComponent(TKey key, int index);
}
