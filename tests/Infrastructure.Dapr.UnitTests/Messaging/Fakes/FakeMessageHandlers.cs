using CodeArchitects.Platform.Infrastructure.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging.Fakes;

[MessageHandler]
public class Message1Handler1 : IMessageHandler<Message1>
{
  public Task HandleAsync(Message1 message, CancellationToken cancellationToken) => throw new NotImplementedException();
}

public class Message1Handler2 : IMessageHandler<Message1>
{
  [MessageHandler]
  public Task HandleAsync(Message1 message, CancellationToken cancellationToken) => throw new NotImplementedException();
}

[MessageHandler(BusName = BusName)]
public class Message1Handler3 : IMessageHandler<Message1>
{
  public const string BusName = nameof(BusName);

  public Task HandleAsync(Message1 message, CancellationToken cancellationToken) => throw new NotImplementedException();
}

[MessageHandler(BusName = BusNameFromType)]
public class Message1Handler4 : IMessageHandler<Message1>
{
  public const string BusNameFromType = nameof(BusNameFromType);
  public const string BusNameFromMethod = nameof(BusNameFromMethod);

  [MessageHandler(BusName = BusNameFromMethod)]
  public Task HandleAsync(Message1 message, CancellationToken cancellationToken) => throw new NotImplementedException();
}

[MessageHandler(Topic = Topic)]
public class Message1Handler5 : IMessageHandler<Message1>
{
  public const string Topic = nameof(Topic);

  public Task HandleAsync(Message1 message, CancellationToken cancellationToken) => throw new NotImplementedException();
}

[MessageHandler(Topic = TopicFromType)]
public class Message1Handler6 : IMessageHandler<Message1>
{
  public const string TopicFromType = nameof(TopicFromType);
  public const string TopicFromMethod = nameof(TopicFromMethod);

  [MessageHandler(Topic = TopicFromMethod)]
  public Task HandleAsync(Message1 message, CancellationToken cancellationToken) => throw new NotImplementedException();
}

[MessageHandler]
public class Message1Handler7 : IMessageHandler<Message1, string>
{
  public Task<string> HandleAsync(Message1 message, CancellationToken cancellationToken) => Task.FromResult(string.Empty);
}

[MessageHandler]
public class Message1And2Handler : IMessageHandler<Message1>, IMessageHandler<Message2, string>
{
  public Task HandleAsync(Message1 message, CancellationToken cancellationToken) => throw new NotImplementedException();

  public Task<string> HandleAsync(Message2 message, CancellationToken cancellationToken) => throw new NotImplementedException();
}

public class ThisHandlerShouldntBeRegistered : IMessageHandler<Message1>
{
  public Task HandleAsync(Message1 message, CancellationToken cancellationToken) => throw new NotImplementedException();
}
