using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public enum CollectionKind
{
  None,
  List,
  HashSet,
  Unknown
}
