using CodeArchitects.Platform.Messaging;

namespace Subscriber.NoResult;

[MessageHandler]
public class ByReflectionNoResultMessageHandler : IMessageHandler<NoResultMessage>
{
  private readonly MessageAwaiter _awaiter;

  public ByReflectionNoResultMessageHandler(MessageAwaiter awaiter)
  {
    _awaiter = awaiter;
  }

  [MessageHandler(Topic = "by-reflection")]
  public Task HandleAsync(NoResultMessage message, CancellationToken cancellationToken)
  {
    _awaiter.Complete(message.Id);
    return Task.CompletedTask;
  }
}

public class ByConfigurationNoResultMessageHandler : IMessageHandler<NoResultMessage>
{
  private readonly MessageAwaiter _awaiter;

  public ByConfigurationNoResultMessageHandler(MessageAwaiter awaiter)
  {
    _awaiter = awaiter;
  }

  public Task HandleAsync(NoResultMessage message, CancellationToken cancellationToken)
  {
    _awaiter.Complete(message.Id);
    return Task.CompletedTask;
  }
}
