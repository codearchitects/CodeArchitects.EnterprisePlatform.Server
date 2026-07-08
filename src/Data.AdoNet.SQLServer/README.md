# CodeArchitects.Platform.Data.AdoNet.SQLServer

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Data.AdoNet.SQLServer.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Data.AdoNet.SQLServer)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

**Microsoft SQL Server** provider for the [`CodeArchitects.Platform.Data.AdoNet`](../Data.AdoNet)
Data Access Layer. Add it alongside the ADO.NET DI package to run repositories against SQL Server.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Data.AdoNet.SQLServer
```

## Setup

```csharp
builder.Services.AddData(cfg => cfg
    .UseSqlServer(builder.Configuration.GetConnectionString("Default")));
```

## Related packages

- [`CodeArchitects.Platform.Data.AdoNet`](../Data.AdoNet) — ADO.NET implementation
- [`CodeArchitects.Platform.Data.AdoNet.DependencyInjection`](../Data.AdoNet.DependencyInjection) — DI registration

## License

Licensed under the [Apache License 2.0](../../LICENSE).
