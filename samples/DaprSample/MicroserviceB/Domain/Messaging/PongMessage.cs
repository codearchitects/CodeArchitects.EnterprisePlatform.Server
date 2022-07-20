using CodeArchitects.Platform.Messaging;

namespace MicroserviceB.Domain.Messaging;

[Message]
public record PongMessage(Guid Id, int Counter);
