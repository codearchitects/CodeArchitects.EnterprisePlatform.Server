using CodeArchitects.Platform.Messaging;
using CodeArchitects.Platform.Messaging.Dapr.Bindings;
using MicroserviceA.Domain.Messaging;

namespace MicroserviceA.Infrastructure.Handlers;

[MessageHandler("messagebus", "pong")]
public class PingPongHandler : IMessageHandler<PongMessage, PingMessage>
{
  [return: MessageBus("messagebus", "ping")]
  [return: StateStore("statestore", "ping")]
  public async Task<PingMessage> HandleAsync(PongMessage message, CancellationToken cancellationToken)
  {
    Console.WriteLine($"[{DateTime.Now}] Pong! {message}");

    await Task.Delay(1000, cancellationToken);

    return new PingMessage(message.Id, message.Counter + 1);
  }
}
