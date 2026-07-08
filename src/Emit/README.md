# CodeArchitects.Platform.Emit

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Emit.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Emit)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

Low-level helpers for **runtime code generation** with `System.Reflection.Emit`. It wraps the raw
IL-emission APIs (`ILGenerator`, `TypeBuilder`, `AssemblyBuilder`) behind more ergonomic, testable
abstractions, and is used internally by the platform to build dynamic proxies and types.

> This is an advanced, low-level package. Most applications will never reference it directly — it is
> a building block for other Code Architects Platform libraries.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Emit
```

## What's inside

| Type | Purpose |
|------|---------|
| `DynamicAssembly` | Creates and manages an in-memory dynamic assembly. |
| `IILGenerator` / `ILGeneratorExtensions` | Fluent, higher-level IL emission (`LoadArg`, `LoadArgs`, labels, locals). |
| `TypeBuilderExtensions` | Helpers such as `DefineMethodOverrideFromDeclaration`. |
| `AssemblyBuilderExtensions` | Helpers such as `IgnoreAccessChecksTo`. |
| `Testing` helpers | Fakes (e.g. `FakeLabelMarker`) to unit-test emitted IL. |

## License

Licensed under the [Apache License 2.0](../../LICENSE).
