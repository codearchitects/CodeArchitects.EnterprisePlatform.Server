using CodeArchitects.Platform.Infrastructure.Messaging;
using System;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging.Fakes;

public record Message1(Guid Id);
public record Message2(Guid Id);

[Message]
public record Message3(Guid Id);