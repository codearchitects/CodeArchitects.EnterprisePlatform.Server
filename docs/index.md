# Code Architects Enterprise Solution Platform

The **Code Architects Enterprise Solution Platform (ESP)** is a set of .NET libraries for building
distributed, cloud-native applications. It provides consistent building blocks for messaging, state
management, data access, actors and cross-cutting policies, with first-class support for
[Dapr](https://dapr.io/).

## Guides

- **Messaging**
  - [Overview](messaging.md) — the publish/subscribe abstractions
  - [With Dapr](messaging-dapr.md) — the Dapr pub/sub implementation
- **State management**
  - [Overview](statemanagement.md) — the `IStateStore` abstraction
  - [With Dapr](statemanagement-dapr.md) — the Dapr state store implementation
- **Data access**
  - [Overview](dataaccesslayer.md) — Repository and Unit of Work patterns
  - [Entity Framework Core](efcore.md) — the EF Core implementation
  - [ADO.NET](adonet.md) — the ADO.NET implementation

## Package reference

Every package ships with its own README describing its purpose and usage. Browse them under
[`src/`](https://github.com/codearchitects/CodeArchitects.EnterprisePlatform.Server/tree/develop/src)
or on [NuGet](https://www.nuget.org/packages?q=CodeArchitects.Platform).

## Getting started

```bash
dotnet add package CodeArchitects.Platform.Messaging
dotnet add package CodeArchitects.Platform.Data
```

See the repository [README](https://github.com/codearchitects/CodeArchitects.EnterprisePlatform.Server#readme)
and the runnable examples under
[`samples/`](https://github.com/codearchitects/CodeArchitects.EnterprisePlatform.Server/tree/develop/samples).
