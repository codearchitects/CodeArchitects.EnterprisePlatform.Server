using CodeArchitects.Platform.Messaging;
using CodeArchitects.Platform.Messaging.Dapr.Bindings;
using MicroserviceA.Domain.Messaging;
using OneOf;

namespace MicroserviceA.Infrastructure.Handlers;

[MessageHandler("messagebus")]
public class PingPongHandler : IMessageHandler<PongMessage, OneOf<PingMessage, PointMessage>>, IMessageHandler<PointMessage>
{
  [MessageHandler(Topic = "pong")]
  [return: MessageBus(typeof(PingMessage), Topic = "ping")]
  [return: MessageBus(typeof(PointMessage), Topic = "point")]
  public async Task<OneOf<PingMessage, PointMessage>> HandleAsync(PongMessage message, CancellationToken cancellationToken)
  {
    Console.WriteLine($"[{DateTime.Now}] Pong! {message.Counter} hits.");

    await Task.Delay(1000, cancellationToken);

    if (Random.Shared.Next(10) == 2)
      return new PointMessage(message.Id, message.Counter + 1, "MicroserviceA");

    return new PingMessage(message.Id, message.Counter + 1);
  }

  [MessageHandler(Topic = "point")]
  public Task HandleAsync(PointMessage message, CancellationToken cancellationToken)
  {
    Console.WriteLine($"[{DateTime.Now}] Point! {message.Winner} wins after {message.Counter} hits!");

    return Task.CompletedTask;
  }
}
