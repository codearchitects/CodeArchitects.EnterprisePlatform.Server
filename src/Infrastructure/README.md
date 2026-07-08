# CodeArchitects.Platform.Infrastructure

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Infrastructure.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Infrastructure)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

Technology-agnostic **state management** abstractions. The central contract is `IStateStore`, a
simple key-value API for building stateful services. This package defines the contract only; use an
implementation such as [`CodeArchitects.Platform.Infrastructure.Dapr`](../Infrastructure.Dapr).

## Installation

```bash
dotnet add package CodeArchitects.Platform.Infrastructure
```

## `IStateStore`

Keys are strings; values can be of any serializable type.

```csharp
public class StatefulClass
{
    private readonly IStateStore _store;

    public StatefulClass(IStateStore store) => _store = store;

    public async Task RunAsync()
    {
        await _store.SaveAsync("key", new MyState { Number = 1 });
        MyState state = await _store.GetAsync<MyState>("key");
        await _store.DeleteAsync("key");
    }
}
```

All methods accept an optional `CancellationToken`.

## Implementations

- [`CodeArchitects.Platform.Infrastructure.Dapr`](../Infrastructure.Dapr) — Dapr state store implementation

## License

Licensed under the [Apache License 2.0](../../LICENSE).
