using CodeArchitects.Platform.Messaging;

namespace Subscriber.WithResult;

[MessageHandler]
public class ByAttributeWithResultMessageHandler : IMessageHandler<WithResultMessage, Result>
{
  [MessageHandler(Topic = "by-attribute")]
  [return: TestOutputBinding("data", typeof(object))]
  public Task<Result> HandleAsync(WithResultMessage message, CancellationToken cancellationToken)
  {
    return Task.FromResult(new Result(message.Id));
  }
}

public class ByConfigurationWithResultMessageHandler : IMessageHandler<WithResultMessage, Result>
{
  public Task<Result> HandleAsync(WithResultMessage message, CancellationToken cancellationToken)
  {
    return Task.FromResult(new Result(message.Id));
  }
}
