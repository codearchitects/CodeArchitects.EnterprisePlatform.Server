using System;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Messaging.Fakes;

public record Message1(Guid Id);
public record Message2(Guid Id);
public record Message3(Guid Id, string Prop) : Message1(Id);