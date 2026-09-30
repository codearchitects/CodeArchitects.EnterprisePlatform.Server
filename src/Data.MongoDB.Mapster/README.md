# CodeArchitects.Platform.Data.MongoDB.Mapster

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Data.MongoDB.Mapster.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Data.MongoDB.Mapster)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

A MongoDB mapped repository that maps between the domain entity and the persisted document with
[Mapster](https://github.com/MapsterMapper/Mapster), for the [`CodeArchitects.Platform.Data.MongoDB`](../Data.MongoDB)
Data Access Layer. Use it when the document must differ from the domain model: renamed or flattened
fields, value objects, technical fields, schema versioning.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Data.MongoDB.Mapster
```

> The package is marked `[Experimental]`, like the MongoDB provider it builds on.

## Setup

```csharp
builder.Services
  .AddData(cfg => cfg
    .UseConnectionString(connectionString)
    .UseDatabase("store")
    .AddEntitiesFrom(typeof(ProductDocument).Assembly))    // [Collection] is on the document
  .AddDataMapster(typeof(ProductMapping).Assembly)          // IRegister implementations
  .AddScoped<IRepository<Product, Guid>, MongoDBMapsterRepository<ProductDocument, Product, Guid>>();
```

```csharp
public sealed class ProductMapping : IRegister
{
  public void Register(TypeAdapterConfig config) =>
    config.NewConfig<Product, ProductDocument>()
      .TwoWays()
      .Map(document => document.Code, product => product.Sku);
}
```

`AddDataMapster` (from [`CodeArchitects.Platform.Data.Mapster`](../Data.Mapster)) validates the whole
configuration at startup, in strict mode. `MongoDBMapsterRepository` also checks, when it is created, that
both directions of its pair have a rule.

## Custom queries

Derive from `MongoDBMapsterRepository` and write the query on the document, passing `Session`:

```csharp
public class ProductRepository(IDataContext context, IDataMapper mapper)
  : MongoDBMapsterRepository<ProductDocument, Product, Guid>(context, mapper), IProductRepository
{
  public Task<List<Product>> FindByCodeAsync(string code, CancellationToken ct = default) =>
    ToEntitiesAsync(Documents.Find(Session, document => document.Code == code), ct);
}
```

Transactions, concurrency checks and exceptions are the same as the direct `MongoDBRepository`. See
[the documentation](../../docs/mongodb.md#mapped-repository) for keys, aggregates, filters and `Include`.

## Related packages

- [`CodeArchitects.Platform.Data.MongoDB`](../Data.MongoDB) — the MongoDB provider, without Mapster
- [`CodeArchitects.Platform.Data.Mapster`](../Data.Mapster) — `AddDataMapster` and the Mapster integration

## License

Licensed under the [Apache License 2.0](../../LICENSE).
