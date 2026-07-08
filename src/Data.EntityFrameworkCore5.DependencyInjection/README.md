# CodeArchitects.Platform.Data.EntityFrameworkCore5.DependencyInjection

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Data.EntityFrameworkCore5.DependencyInjection.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Data.EntityFrameworkCore5.DependencyInjection)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

Dependency-injection wiring for the
[`CodeArchitects.Platform.Data.EntityFrameworkCore5`](../Data.EntityFrameworkCore5) Data Access Layer
(EF Core 5, for .NET Core 3.1 / .NET 5). Registers the data context, repositories and unit of work
via `AddData`.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Data.EntityFrameworkCore5.DependencyInjection
```

## Setup

```csharp
services.AddData<MyDbContext>(cfg => cfg
    .UseSqlServer(configuration.GetConnectionString("Default")));
```

## Related packages

- [`CodeArchitects.Platform.Data.EntityFrameworkCore5`](../Data.EntityFrameworkCore5) — EF Core 5 implementation
- [`CodeArchitects.Platform.Data`](../Data) — DAL abstractions

## License

Licensed under the [Apache License 2.0](../../LICENSE).
