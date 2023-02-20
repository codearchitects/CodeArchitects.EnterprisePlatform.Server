using CodeArchitects.Platform.Actors;

namespace ActorApp.Domain;

[ActorFactory<TestActor>]
public interface ITestActorFactory // TODO: Delete when analyzer is ready
{
  ITestActor Get(Guid id);
}
