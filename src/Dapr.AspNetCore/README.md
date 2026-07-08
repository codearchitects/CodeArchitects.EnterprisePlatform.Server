# CodeArchitects.Platform.Dapr.AspNetCore

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Dapr.AspNetCore.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Dapr.AspNetCore)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

The entry point for using [Dapr](https://dapr.io/) building blocks in an ASP.NET Core application.
It provides `AddDaprInfrastructure`, which returns an `IDaprInfrastructureBuilder` that the other
Dapr packages (messaging, state, actors) extend with a fluent API.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Dapr.AspNetCore
```

## Setup

```csharp
builder.Services
    .AddDaprInfrastructure(cfg => cfg.AddServiceOptions(builder.Configuration))
    // then chain the building blocks you need, e.g.:
    .AddMessageBus()
    .AddMessageHandlers()
    .AddStateStore()
    .AddActors();
```

`AddDaprInfrastructure` has two overloads:

- one that takes a `DaprConfig` instance directly — recommended for prototyping and testing;
- one that takes a building action to compose configuration from `IConfiguration` and other sources —
  recommended for production.

## Key types

| Type | Purpose |
|------|---------|
| `IDaprInfrastructureBuilder` | Fluent builder that other Dapr packages extend. |
| `DaprConfig` | Root configuration for the Dapr infrastructure. |
| `IDaprComponentAccessor` | Access to configured Dapr components and their metadata. |

## Related packages

- [`CodeArchitects.Platform.Messaging.Dapr.AspNetCore`](../Messaging.Dapr.AspNetCore)
- [`CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore`](../Infrastructure.Dapr.AspNetCore)
- [`CodeArchitects.Platform.Actors.Dapr.AspNetCore`](../Actors.Dapr.AspNetCore)

## License

Licensed under the [Apache License 2.0](../../LICENSE).
