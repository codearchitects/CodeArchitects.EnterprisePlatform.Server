namespace CodeArchitects.Platform.Actors.Metadata;

internal interface IActorIdDescriptor
{
  Type Type { get; }

  bool HasIdSource { get; }
  
  int StateIndex { get; }
}
