using CodeArchitects.Platform.Messaging;

namespace MicroserviceA.Domain.Messaging;

[Message]
public record PingMessage(Guid Id, int Counter);
