using CodeArchitects.Platform.Messaging;

namespace MicroserviceA.Domain.Messaging;

[Message]
public record PointMessage(Guid Id, int Counter, string Winner);
