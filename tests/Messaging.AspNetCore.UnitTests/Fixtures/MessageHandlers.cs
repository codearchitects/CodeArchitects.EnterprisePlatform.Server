namespace CodeArchitects.Platform.Messaging.AspNetCore.Fixtures;

public class Message1Handler : IMessageHandler<Message1>
{
  public virtual Task HandleAsync(Message1 message, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}

public class Message2Handler : IMessageHandler<Message2, object>
{
  public virtual Task<object> HandleAsync(Message2 message, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
