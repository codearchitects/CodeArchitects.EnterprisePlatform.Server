namespace ActorApp.Domain;

public interface ITrafficLight
{
  Task StartAsync(CancellationToken cancellationToken = default);

  Task StopAsync(CancellationToken cancellationToken = default);

  ValueTask<LightColor> GetLightColorAsync(CancellationToken cancellationToken = default);

  Task<TrafficLightResponse> CrossIntersectionAsync(CancellationToken cancellationToken = default);
}