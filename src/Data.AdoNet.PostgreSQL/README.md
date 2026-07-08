# CodeArchitects.Platform.Data.AdoNet.PostgreSQL

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Data.AdoNet.PostgreSQL.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Data.AdoNet.PostgreSQL)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

**PostgreSQL** provider for the [`CodeArchitects.Platform.Data.AdoNet`](../Data.AdoNet) Data Access
Layer. Add it alongside the ADO.NET DI package to run repositories against PostgreSQL.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Data.AdoNet.PostgreSQL
```

## Setup

```csharp
builder.Services.AddData(cfg => cfg
    .UsePostgreSQL(builder.Configuration.GetConnectionString("Default")));
```

## Related packages

- [`CodeArchitects.Platform.Data.AdoNet`](../Data.AdoNet) — ADO.NET implementation
- [`CodeArchitects.Platform.Data.AdoNet.DependencyInjection`](../Data.AdoNet.DependencyInjection) — DI registration

## License

Licensed under the [Apache License 2.0](../../LICENSE).
