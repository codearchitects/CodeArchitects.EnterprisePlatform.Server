# CodeArchitects.Platform.Messaging.Dapr

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Messaging.Dapr.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Messaging.Dapr)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

[Dapr](https://dapr.io/) pub/sub implementation of the [`CodeArchitects.Platform.Messaging`](../Messaging)
abstractions. It wraps the Dapr pub/sub building block so you can publish and subscribe to messages
without dealing with the Dapr API directly.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Messaging.Dapr
```

## Metadata

Use the strongly-typed `DaprMetadata` to control per-message Dapr options:

```csharp
public class Publisher
{
    private readonly IMessageBus<DaprMetadata> _bus;

    public Publisher(IMessageBus<DaprMetadata> bus) => _bus = bus;

    public Task PublishAsync(MyMessage message) =>
        _bus.SendAsync(message, new DaprMetadata
        {
            TimeToLiveInSeconds = 30,
            RawPayload = true,
            Topic = "myTopic"
        });
}
```

| `DaprMetadata` property | Description |
|-------------------------|-------------|
| `TimeToLiveInSeconds` (`int?`) | Message expiration, in seconds. |
| `RawPayload` (`bool?`) | Publish without CloudEvent wrapping. |
| `Topic` (`string?`) | Target topic (defaults to the message type name). |

## Related packages

- [`CodeArchitects.Platform.Messaging`](../Messaging) — core abstractions
- [`CodeArchitects.Platform.Messaging.Dapr.AspNetCore`](../Messaging.Dapr.AspNetCore) — DI + endpoint mapping for ASP.NET Core

## License

Licensed under the [Apache License 2.0](../../LICENSE).
