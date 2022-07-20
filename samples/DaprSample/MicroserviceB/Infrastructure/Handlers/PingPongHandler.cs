using CodeArchitects.Platform.Messaging;
using CodeArchitects.Platform.Messaging.Dapr.Bindings;
using MicroserviceB.Domain.Messaging;

namespace MicroserviceB.Infrastructure.Handlers;

[MessageHandler("messagebus", "ping")]
public class PingPongHandler : IMessageHandler<PingMessage, PongMessage>
{
  [return: MessageBus("messagebus", "pong")]
  public async Task<PongMessage> HandleAsync(PingMessage message, CancellationToken cancellationToken)
  {
    Console.WriteLine($"[{DateTime.Now}] Ping! {message}");

    await Task.Delay(1000, cancellationToken);

    return new PongMessage(message.Id, message.Counter + 1);
  }
}
