# CodeArchitects.Platform.Actors.Dapr

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Actors.Dapr.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Actors.Dapr)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

[Dapr Actors](https://docs.dapr.io/developing-applications/building-blocks/actors/) implementation of
the [`CodeArchitects.Platform.Actors`](../Actors) abstractions. It maps the platform actor model onto
Dapr's virtual-actor runtime, which transparently handles activation, placement, turn-based
concurrency and state persistence.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Actors.Dapr
```

## Related packages

- [`CodeArchitects.Platform.Actors`](../Actors) — actor model abstractions
- [`CodeArchitects.Platform.Actors.Dapr.AspNetCore`](../Actors.Dapr.AspNetCore) — DI + endpoint registration

## License

Licensed under the [Apache License 2.0](../../LICENSE).
