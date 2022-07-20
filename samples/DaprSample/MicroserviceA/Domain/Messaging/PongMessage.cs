using CodeArchitects.Platform.Messaging;

namespace MicroserviceA.Domain.Messaging;

[Message]
public record PongMessage(Guid Id, int Counter);
