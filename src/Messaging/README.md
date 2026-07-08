# CodeArchitects.Platform.Messaging

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Messaging.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Messaging)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

Technology-agnostic **publish/subscribe messaging** abstractions for building distributed
applications. This package defines the contracts only; pick an implementation package such as
[`CodeArchitects.Platform.Messaging.Dapr`](../Messaging.Dapr) to actually send and receive messages.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Messaging
```

## Key abstractions

| Type | Purpose |
|------|---------|
| `IMessageBus` | Publishes messages (`SendAsync`). |
| `IMessageBus<TMetadata>` | Publishes messages with implementation-specific metadata. |
| `IMessageHandler<TMessage>` | Subscribes to and handles a message type (`HandleAsync`). |
| `IOutputBinding` / `IOutputMetadata` | Output binding abstractions for one-way integrations. |

## Messages

Messages are plain POCOs (or `record`s) — no marker interface required:

```csharp
public record MyMessage(Guid Id, int SequenceNumber, string Content);
```

## Publishing

```csharp
public class Publisher
{
    private readonly IMessageBus _bus;

    public Publisher(IMessageBus bus) => _bus = bus;

    public Task PublishAsync() =>
        _bus.SendAsync(new MyMessage(Guid.NewGuid(), 1, "hello"));
}
```

## Subscribing

```csharp
public class MyMessageHandler : IMessageHandler<MyMessage>
{
    public Task HandleAsync(MyMessage message, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Received message {message.Id}");
        return Task.CompletedTask;
    }
}
```

## Implementations

- [`CodeArchitects.Platform.Messaging.Dapr`](../Messaging.Dapr) — Dapr pub/sub implementation
- [`CodeArchitects.Platform.Messaging.AspNetCore`](../Messaging.AspNetCore) — ASP.NET Core hosting/endpoints

## License

Licensed under the [Apache License 2.0](../../LICENSE).
