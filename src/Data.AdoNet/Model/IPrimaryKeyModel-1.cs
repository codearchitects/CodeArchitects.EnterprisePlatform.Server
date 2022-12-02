using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IPrimaryKeyModel<TKey> : IPrimaryKeyModel
  where TKey : IEquatable<TKey>
{
  new Getter<TKey> GetValue { get; }

  new Setter<TKey> SetValue { get; }

  object GetKeyComponent(TKey key, int index);
}
