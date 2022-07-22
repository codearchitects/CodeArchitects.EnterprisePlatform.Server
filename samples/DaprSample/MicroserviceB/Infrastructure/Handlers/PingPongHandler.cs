using CodeArchitects.Platform.Messaging;
using MicroserviceB.Domain.Messaging;
using OneOf;

namespace MicroserviceB.Infrastructure.Handlers;

public class PingPongHandler : IMessageHandler<PingMessage, OneOf<PongMessage, PointMessage>>, IMessageHandler<PointMessage>
{
  public async Task<OneOf<PongMessage, PointMessage>> HandleAsync(PingMessage message, CancellationToken cancellationToken)
  {
    Console.WriteLine($"[{DateTime.Now}] Ping! {message.Counter} hits.");

    await Task.Delay(1000, cancellationToken);

    if (Random.Shared.Next(10) == 2)
      return new PointMessage(message.Id, message.Counter + 1, "MicroserviceA");

    return new PongMessage(message.Id, message.Counter + 1);
  }

  public Task HandleAsync(PointMessage message, CancellationToken cancellationToken)
  {
    Console.WriteLine($"[{DateTime.Now}] Point! {message.Winner} wins after {message.Counter} hits!");

    return Task.CompletedTask;
  }
}
