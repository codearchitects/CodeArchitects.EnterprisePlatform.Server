# CodeArchitects.Platform.Data.AdoNet.DependencyInjection

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Data.AdoNet.DependencyInjection.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Data.AdoNet.DependencyInjection)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

Dependency-injection wiring for the [`CodeArchitects.Platform.Data.AdoNet`](../Data.AdoNet) Data
Access Layer. It registers the ADO.NET data context, repositories and unit of work in the ASP.NET
Core service container via `AddData`, and lets you select the database provider fluently.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Data.AdoNet.DependencyInjection
```

Also add the provider package for your database (SQL Server, PostgreSQL, MySQL or Oracle).

## Setup

```csharp
builder.Services.AddData(cfg => cfg
    .UseSqlServer(builder.Configuration.GetConnectionString("Default")));
// register your specialized repositories, e.g.:
builder.Services.AddScoped<IProductRepository, ProductRepository>();
```

Use the corresponding `Use*` method for your provider (`UsePostgreSQL`, `UseMySQL`, `UseOracle`, …),
provided by the respective provider package.

## Related packages

- [`CodeArchitects.Platform.Data.AdoNet`](../Data.AdoNet) — ADO.NET implementation
- Providers: [SQL Server](../Data.AdoNet.SQLServer) · [PostgreSQL](../Data.AdoNet.PostgreSQL) · [MySQL](../Data.AdoNet.MySQL) · [Oracle](../Data.AdoNet.Oracle)

## License

Licensed under the [Apache License 2.0](../../LICENSE).
