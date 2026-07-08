# CodeArchitects.Platform.Actors.Dapr.AspNetCore

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Actors.Dapr.AspNetCore.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Actors.Dapr.AspNetCore)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

Dependency-injection and hosting support for running [Dapr Actors](https://dapr.io/) in an ASP.NET
Core application, on top of [`CodeArchitects.Platform.Actors.Dapr`](../Actors.Dapr).

## Installation

```bash
dotnet add package CodeArchitects.Platform.Actors.Dapr.AspNetCore
```

## Setup

Register actors on the shared Dapr infrastructure builder:

```csharp
builder.Services
    .AddDaprInfrastructure(cfg => cfg.AddServiceOptions(builder.Configuration))
    .AddActors();
```

## Related packages

- [`CodeArchitects.Platform.Actors`](../Actors) — actor model abstractions
- [`CodeArchitects.Platform.Actors.Dapr`](../Actors.Dapr) — Dapr Actors implementation
- [`CodeArchitects.Platform.Dapr.AspNetCore`](../Dapr.AspNetCore) — `AddDaprInfrastructure` host builder

## License

Licensed under the [Apache License 2.0](../../LICENSE).
