using CodeArchitects.Platform.Infrastructure.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Fakes;

[MessageHandler]
public class Message1Handler : IMessageHandler<Message1>
{
  public Task HandleAsync(Message1 message, CancellationToken cancellationToken) => throw new NotImplementedException();
}

[MessageHandler]
public class Message2Handler : IMessageHandler<Message2, string>
{
  public Task<string> HandleAsync(Message2 message, CancellationToken cancellationToken) => throw new NotImplementedException();
}