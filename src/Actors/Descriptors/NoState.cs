using CodeArchitects.Platform.Actors.Infrastructure;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal class NoState : OrdinaryActorState
{
  public static readonly NoState Instance = new NoState();

  private NoState() { }
}
