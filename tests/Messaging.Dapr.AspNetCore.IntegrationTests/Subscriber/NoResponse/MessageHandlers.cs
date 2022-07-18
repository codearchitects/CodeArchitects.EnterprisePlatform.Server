using CodeArchitects.Platform.Messaging;

namespace Subscriber.NoResponse;

[MessageHandler]
public class ByReflectionNoResponseMessageHandler : IMessageHandler<NoResponseMessage>
{
  private readonly NoResponseAwaiter _awaiter;

  public ByReflectionNoResponseMessageHandler(NoResponseAwaiter awaiter)
  {
    _awaiter = awaiter;
  }

  [MessageHandler(Topic = "by-reflection")]
  public Task HandleAsync(NoResponseMessage message, CancellationToken cancellationToken)
  {
    _awaiter.Complete(message.Id);
    return Task.CompletedTask;
  }
}

public class ByConfigurationNoResponseMessageHandler : IMessageHandler<NoResponseMessage>
{
  private readonly NoResponseAwaiter _awaiter;

  public ByConfigurationNoResponseMessageHandler(NoResponseAwaiter awaiter)
  {
    _awaiter = awaiter;
  }

  public Task HandleAsync(NoResponseMessage message, CancellationToken cancellationToken)
  {
    _awaiter.Complete(message.Id);
    return Task.CompletedTask;
  }
}
