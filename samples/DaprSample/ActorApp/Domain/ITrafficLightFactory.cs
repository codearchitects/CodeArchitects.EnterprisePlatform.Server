using CodeArchitects.Platform.Actors;

namespace ActorApp.Domain;

[ActorFactory<TrafficLight>]
public interface ITrafficLightFactory // TODO: Delete when analyzer is ready
{
  ITrafficLight Get(Guid id);
}
