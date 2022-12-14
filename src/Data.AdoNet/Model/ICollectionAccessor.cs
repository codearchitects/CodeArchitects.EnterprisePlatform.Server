using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface ICollectionAccessor
{
  void Add(object instance, object value);

  void Remove(object instance, object value);

  bool TryGetNonEnumeratedCount(object instance, out int count);
}
