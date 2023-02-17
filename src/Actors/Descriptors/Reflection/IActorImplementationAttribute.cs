namespace CodeArchitects.Platform.Actors.Descriptors.Reflection;

internal interface IActorImplementationAttribute
{
  Type ActorType { get; }

  bool IsDefault { get; }
}
