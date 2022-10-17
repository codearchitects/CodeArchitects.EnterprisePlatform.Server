using CodeArchitects.Platform.Messaging;

namespace MicroserviceB.Domain.Messaging;

[Message]
public record PingMessage(Guid Id, int Counter);
