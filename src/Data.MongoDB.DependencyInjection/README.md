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
    .UseConnectionString(builder.Configuration.GetConnectionString("Mongo")!)
    .UseDatabase("mydb")
    .AddEntitiesFrom(typeof(Product).Assembly));
// register your specialized repositories, e.g.:
builder.Services.AddScoped<IProductRepository, ProductRepository>();
```

The fluent builder guides you from client (`IMongoDBConfigurationBuilderWithClient`) to database
(`IMongoDBConfigurationBuilderWithDatabase`), where the entity model and the provider behaviour
are configured. `AddData` lives in the `Microsoft.Extensions.DependencyInjection` namespace, like
the other providers, so no extra `using` is needed.

Entities are discovered by scanning an assembly for public, concrete classes marked with
`[Collection]` (`[Table]` is also accepted); `AddEntity<TEntity>()` registers one explicitly. The
configuration is validated eagerly — a missing client or database, an empty model, an
unresolvable key or two entities mapped to the same collection all fail at startup, without
contacting the server.

## Options

```csharp
builder.Services.AddData(cfg => cfg
    .UseClient(sp => sp.GetRequiredService<IMongoClient>())   // bring your own client
    .UseDatabase("mydb")
    .AddEntitiesFrom(typeof(Product).Assembly)
    .UseTransactions(TransactionMode.Required)               // default
    .UseGuidRepresentation(GuidRepresentation.Standard)      // default
    .ConfigureConventions(pack => pack.Add(new CamelCaseElementNameConvention()))
    .UseSeed<ApplicationDataSeed>());
```

`UseTransactions` controls what happens when an operation needs a multi-document transaction.
MongoDB provides them only on a replica set or a sharded cluster: with the default `Required` the
operation fails explicitly on a standalone server, `WhenSupported` degrades with a warning, and
`Disabled` never opens one (local development).

## Related packages

- [`CodeArchitects.Platform.Data.MongoDB`](../Data.MongoDB) — MongoDB implementation
- [`CodeArchitects.Platform.Data`](../Data) — DAL abstractions

## License

Licensed under the [Apache License 2.0](../../LICENSE).
