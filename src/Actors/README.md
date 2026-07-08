# CodeArchitects.Platform.Actors

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Actors.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Actors)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

Technology-agnostic **actor model** abstractions for the Code Architects Platform. Actors are
single-threaded, stateful objects that process messages one at a time, making them a natural fit for
concurrent, distributed domain logic. This package defines the contracts; use an implementation such
as [`CodeArchitects.Platform.Actors.Dapr`](../Actors.Dapr) to run them.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Actors
```

## Key abstractions

| Type | Purpose |
|------|---------|
| `IActorContext` | Ambient context of the currently executing actor (identity, state). |
| `IActorMessage` | Marker for messages exchanged with actors. |
| `ActorConfiguration` | Configuration of an actor type and its behavior. |
| `ActorModelFactory` | Builds the actor model from your actor definitions. |

## Related packages

- [`CodeArchitects.Platform.Actors.Dapr`](../Actors.Dapr) — Dapr Actors implementation
- [`CodeArchitects.Platform.Actors.Dapr.AspNetCore`](../Actors.Dapr.AspNetCore) — ASP.NET Core hosting

## License

Licensed under the [Apache License 2.0](../../LICENSE).
