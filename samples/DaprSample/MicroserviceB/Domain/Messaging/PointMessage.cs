using CodeArchitects.Platform.Messaging;

namespace MicroserviceB.Domain.Messaging;

[Message]
public record PointMessage(Guid Id, int Counter, string Winner);
