# CodeArchitects.Platform.Messaging.Dapr.AspNetCore

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Messaging.Dapr.AspNetCore.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Messaging.Dapr.AspNetCore)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

Dependency-injection and endpoint wiring to use [Dapr](https://dapr.io/) pub/sub messaging in an
ASP.NET Core application. This is the package you reference in a service that publishes and/or
consumes messages via Dapr.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Messaging.Dapr.AspNetCore
```

## Configuration

```json
{
  "Messaging": {
    "DefaultBus": "pubsub"
  }
}
```

## Setup

Register the Dapr infrastructure, publishing (`AddMessageBus`) and handlers (`AddMessageHandlers`):

```csharp
builder.Services
    .AddDaprInfrastructure(cfg => cfg.AddServiceOptions(builder.Configuration))
    .AddMessageBus()
    .AddMessageHandlers(); // scans the calling assembly if none is specified
```

Expose the endpoints the Dapr sidecar calls to deliver messages:

```csharp
app.UseEndpoints(endpoints =>
{
    endpoints.MapMessageHandlers();
});
```

## Routing to topics and buses

```csharp
[SubscribeToTopic("myTopic")]
[SubscribeToBus("myBus")]
public class MyMessageHandler : IMessageHandler<MyMessage>
{
    public Task HandleAsync(MyMessage message, CancellationToken cancellationToken) => Task.CompletedTask;
}
```

When multiple brokers are configured, resolve a specific bus by name via `IServiceResolver<IMessageBus>`.

## Related packages

- [`CodeArchitects.Platform.Messaging`](../Messaging) — core abstractions
- [`CodeArchitects.Platform.Messaging.Dapr`](../Messaging.Dapr) — Dapr implementation & `DaprMetadata`
- [`CodeArchitects.Platform.Dapr.AspNetCore`](../Dapr.AspNetCore) — `AddDaprInfrastructure` host builder

## License

Licensed under the [Apache License 2.0](../../LICENSE).
