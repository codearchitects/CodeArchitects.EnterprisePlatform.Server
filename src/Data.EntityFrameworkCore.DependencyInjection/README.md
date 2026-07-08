# CodeArchitects.Platform.Data.EntityFrameworkCore.DependencyInjection

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Data.EntityFrameworkCore.DependencyInjection.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Data.EntityFrameworkCore.DependencyInjection)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

Dependency-injection wiring for the
[`CodeArchitects.Platform.Data.EntityFrameworkCore`](../Data.EntityFrameworkCore) Data Access Layer.
It registers the EF Core data context, repositories and unit of work via `AddData`.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Data.EntityFrameworkCore.DependencyInjection
```

## Setup

```csharp
builder.Services.AddData<MyDbContext>(cfg => cfg
    .UseSqlServer(builder.Configuration.GetConnectionString("Default")));
// register your specialized repositories, e.g.:
builder.Services.AddScoped<IProductRepository, ProductRepository>();
```

## Related packages

- [`CodeArchitects.Platform.Data.EntityFrameworkCore`](../Data.EntityFrameworkCore) — EF Core implementation
- [`CodeArchitects.Platform.Data`](../Data) — DAL abstractions

## License

Licensed under the [Apache License 2.0](../../LICENSE).
