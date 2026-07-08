# CodeArchitects.Platform.PolicyManager.DependencyInjection

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.PolicyManager.DependencyInjection.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.PolicyManager.DependencyInjection)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

Dependency-injection wiring for [`CodeArchitects.Platform.PolicyManager`](../PolicyManager). Registers
`IPolicyManager` and your policies in the ASP.NET Core service container via `AddPolicyManager`.

## Installation

```bash
dotnet add package CodeArchitects.Platform.PolicyManager.DependencyInjection
```

## Setup

```csharp
builder.Services.AddPolicyManager();
```

## Related packages

- [`CodeArchitects.Platform.PolicyManager`](../PolicyManager) — policy engine

## License

Licensed under the [Apache License 2.0](../../LICENSE).
