using CodeArchitects.Platform.Infrastructure.Dapr.Messaging;
using CodeArchitects.Platform.Infrastructure.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Fakes;

public class FakeMessage1Handler1 : IMessageHandler<FakeMessage1>
{
  public Task HandleAsync(FakeMessage1 message, CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}

public class FakeMessage1Handler2 : IMessageHandler<FakeMessage1>
{
  public Task HandleAsync(FakeMessage1 message, CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}

[SubscribeToTopic(TopicName)]
public class FakeMessage1HandlerWithTopic : IMessageHandler<FakeMessage1>
{
  public const string TopicName = nameof(TopicName);

  public Task HandleAsync(FakeMessage1 message, CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}

[SubscribeToBus(BusName), SubscribeToTopic(TopicName)]
public class FakeMessage1HandlerWithBusAndTopic : IMessageHandler<FakeMessage1>
{
  public const string BusName = nameof(BusName);
  public const string TopicName = nameof(TopicName);

  public Task HandleAsync(FakeMessage1 message, CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}

public class FakeMessage2Handler : IMessageHandler<FakeMessage2>
{
  public Task HandleAsync(FakeMessage2 message, CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}

public class FakeMessage1And2Handler : IMessageHandler<FakeMessage1>, IMessageHandler<FakeMessage2>
{
  public Task HandleAsync(FakeMessage1 message, CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }

  public Task HandleAsync(FakeMessage2 message, CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}
