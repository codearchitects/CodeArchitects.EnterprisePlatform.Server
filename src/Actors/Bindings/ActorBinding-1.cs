namespace CodeArchitects.Platform.Actors.Bindings;

internal class ActorBinding<TActor> : ActorBinding<TActor, TActor>
  where TActor : class
{
  protected override TActor Cast(TActor actor)
  {
    return actor;
  }
}