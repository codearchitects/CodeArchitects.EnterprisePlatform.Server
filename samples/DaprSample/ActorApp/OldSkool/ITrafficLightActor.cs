using Dapr.Actors;

namespace ActorApp.OldSkool;

public interface ITrafficLightActor : IActor
{
  Task TurnOnAsync(CancellationToken cancellationToken = default);

  Task TurnOffAsync(CancellationToken cancellationToken = default);

  Task<string> GetLightColorAsync(CancellationToken cancellationToken = default);

  Task<TrafficLightResponse> CrossIntersectionAsync(CancellationToken cancellationToken = default);
}