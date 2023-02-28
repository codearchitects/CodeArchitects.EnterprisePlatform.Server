using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Factory;

internal interface IStateComponentMetadata
{
  int Index { get; }
  
  MemberInfo Member { get; }
  
  Type Type { get; }
}