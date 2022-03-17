using CodeArchitects.Platform.Infrastructure.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Messaging.Fakes;

[MessageHandler("messagebus")]
public class Message1Handler : IMessageHandler<Message1>
{
  public Task HandleAsync(Message1 message, CancellationToken cancellationToken) => Task.CompletedTask;
}