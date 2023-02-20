using Dapr.Actors;

namespace ActorApp.OldSkool;

public interface ITrafficLightActor : IActor
{
  Task StartAsync(CancellationToken cancellationToken = default);

  Task StopAsync(CancellationToken cancellationToken = default);

  Task<string> GetLightColorAsync(CancellationToken cancellationToken = default);

  Task<TrafficLightResponse> CrossIntersectionAsync(CancellationToken cancellationToken = default);
}