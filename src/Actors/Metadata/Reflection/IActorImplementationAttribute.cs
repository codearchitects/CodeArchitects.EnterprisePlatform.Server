namespace CodeArchitects.Platform.Actors.Metadata.Reflection;

internal interface IActorImplementationAttribute
{
  Type ActorType { get; }

  bool IsDefault { get; }
}
