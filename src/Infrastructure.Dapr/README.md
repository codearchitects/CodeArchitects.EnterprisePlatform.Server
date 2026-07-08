# CodeArchitects.Platform.Infrastructure.Dapr

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Infrastructure.Dapr.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Infrastructure.Dapr)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

[Dapr](https://dapr.io/) state-management implementation of `IStateStore` from
[`CodeArchitects.Platform.Infrastructure`](../Infrastructure). It wraps the Dapr state building block
behind the simple key-value `IStateStore` API.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Infrastructure.Dapr
```

## Related packages

- [`CodeArchitects.Platform.Infrastructure`](../Infrastructure) — `IStateStore` abstraction
- [`CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore`](../Infrastructure.Dapr.AspNetCore) — DI registration

## License

Licensed under the [Apache License 2.0](../../LICENSE).
