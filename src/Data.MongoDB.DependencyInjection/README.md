# CodeArchitects.Platform.Data.MongoDB.DependencyInjection

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Data.MongoDB.DependencyInjection.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Data.MongoDB.DependencyInjection)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

Dependency-injection wiring for the [`CodeArchitects.Platform.Data.MongoDB`](../Data.MongoDB) Data
Access Layer. Registers the MongoDB client, database, data context and repositories via `AddData`.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Data.MongoDB.DependencyInjection
```

## Setup

```csharp
builder.Services.AddData(cfg => cfg
    .UseClient(builder.Configuration.GetConnectionString("Mongo"))
    .UseDatabase("mydb"));
// register your specialized repositories, e.g.:
builder.Services.AddScoped<IProductRepository, ProductRepository>();
```

The fluent builder guides you from client (`IMongoDBConfigurationBuilderWithClient`) to database
(`IMongoDBConfigurationBuilderWithDatabase`).

## Related packages

- [`CodeArchitects.Platform.Data.MongoDB`](../Data.MongoDB) — MongoDB implementation
- [`CodeArchitects.Platform.Data`](../Data) — DAL abstractions

## License

Licensed under the [Apache License 2.0](../../LICENSE).
