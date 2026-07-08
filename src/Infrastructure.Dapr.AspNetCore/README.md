# CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

Dependency-injection support for using [Dapr](https://dapr.io/) state stores in an ASP.NET Core
application, on top of [`CodeArchitects.Platform.Infrastructure.Dapr`](../Infrastructure.Dapr).

## Installation

```bash
dotnet add package CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore
```

## Configuration

```json
{
  "State": {
    "DefaultStore": "statestore"
  }
}
```

## Setup

```csharp
builder.Services
    .AddDaprInfrastructure(cfg => cfg.AddServiceOptions(builder.Configuration))
    .AddStateStore();
```

This registers an `IServiceResolver<IStateStore>` (and, when a default store is configured, a default
`IStateStore`) as singletons. Resolve a specific store by name when using multiple stores:

```csharp
public StatefulClass(IServiceResolver<IStateStore> resolver) =>
    _store = resolver.Resolve("storeName");
```

## Related packages

- [`CodeArchitects.Platform.Infrastructure`](../Infrastructure) — `IStateStore` abstraction
- [`CodeArchitects.Platform.Infrastructure.Dapr`](../Infrastructure.Dapr) — Dapr implementation
- [`CodeArchitects.Platform.Dapr.AspNetCore`](../Dapr.AspNetCore) — `AddDaprInfrastructure` host builder

## License

Licensed under the [Apache License 2.0](../../LICENSE).
