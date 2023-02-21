namespace ActorApp.Domain;

public interface ITrafficLight
{
  Task TurnOnAsync(CancellationToken cancellationToken = default);

  Task TurnOffAsync(CancellationToken cancellationToken = default);

  ValueTask<string> GetLightColorAsync(CancellationToken cancellationToken = default);

  Task<TrafficLightResponse> CrossIntersectionAsync(CancellationToken cancellationToken = default);
}