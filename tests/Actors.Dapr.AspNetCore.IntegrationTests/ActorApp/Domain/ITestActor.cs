namespace ActorApp.Domain;

public interface ITestActor
{
  Task<int> GetStateAsync(CancellationToken cancellationToken = default);
}