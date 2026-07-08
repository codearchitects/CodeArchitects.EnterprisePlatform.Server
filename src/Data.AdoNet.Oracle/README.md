# CodeArchitects.Platform.Data.AdoNet.Oracle

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Data.AdoNet.Oracle.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Data.AdoNet.Oracle)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

**Oracle Database** provider for the [`CodeArchitects.Platform.Data.AdoNet`](../Data.AdoNet) Data
Access Layer. Add it alongside the ADO.NET DI package to run repositories against Oracle.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Data.AdoNet.Oracle
```

## Setup

```csharp
builder.Services.AddData(cfg => cfg
    .UseOracle(builder.Configuration.GetConnectionString("Default")));
```

> This provider depends on `Oracle.ManagedDataAccess.Core`, which is distributed under Oracle's own
> proprietary license — see the package's terms on NuGet.

## Related packages

- [`CodeArchitects.Platform.Data.AdoNet`](../Data.AdoNet) — ADO.NET implementation
- [`CodeArchitects.Platform.Data.AdoNet.DependencyInjection`](../Data.AdoNet.DependencyInjection) — DI registration

## License

Licensed under the [Apache License 2.0](../../LICENSE).
