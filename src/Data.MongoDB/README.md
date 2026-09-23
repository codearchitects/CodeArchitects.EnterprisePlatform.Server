# CodeArchitects.Platform.Data.MongoDB

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Data.MongoDB.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Data.MongoDB)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

A **MongoDB** implementation of the [`CodeArchitects.Platform.Data`](../Data) Data Access Layer,
built on the official MongoDB .NET driver. It provides repository base classes so you consume MongoDB
through the same Repository/Unit of Work API as the relational providers.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Data.MongoDB
```

> The package is marked `[Experimental]`: its public surface may change between minor versions.
> Requires **MongoDB Server 4.4 or later** (a requirement of driver 3.x).

## Declaring an entity

An entity is a public, concrete class marked with `[Collection]`. Its key is always mapped to the
`_id` element, resolved as `[BsonId]`, then `Id`, then `<TypeName>Id`.

```csharp
using CodeArchitects.Platform.Data.MongoDB;

[Collection("products")]
public class Product
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }
}
```

## Implementing a repository

Derive from `MongoDBRepository<TEntity, TKey>`:

```csharp
public class ProductRepository : MongoDBRepository<Product, Guid>, IProductRepository
{
    public ProductRepository(IDataContext context) : base(context) { }

    public Task<List<Product>> GetTopSellingAsync(int count, CancellationToken ct = default) =>
        Collection
            .Find(Session, Builders<Product>.Filter.Empty)
            .SortByDescending(product => product.SaleCount)
            .Limit(count)
            .ToListAsync(ct);
}
```

The base class exposes `Collection`, `Database` and `Session`. **Pass `Session` to your own
queries**: an operation issued without it runs on an implicit session, therefore outside the
transaction of the unit of work in progress.

For a custom document↔domain mapping, derive from `MongoDBMappedRepository<TDocument, TEntity, TKey>`,
which exposes `Collection` (and its alias `Documents`) typed on the *document*. Put `[Collection]`
on the document, never on the domain entity.

## Aggregates

The document is the unit of consistency: an aggregate root is one document, and its intra-aggregate
children are embedded in it — so writing the root is atomic without a transaction. Inter-aggregate
associations are plain key references, never written in cascade. `DBRef` is not supported.

## Seeding

```csharp
[SeedOrder(1)]
public class CategorySeed : DataSeed
{
    public override void Seed(ISeeder seeder) => seeder.Seed(new Category { ... });
}

app.Services.SeedMongo();   // or await app.Services.SeedMongoAsync()
```

Seeding is idempotent per collection and commits every seed in a single transaction.

## Not supported yet

`Include` of references between collections (embedded navigations are supported and already
loaded with the document), optimistic concurrency, multitenancy, soft delete, change tracking,
composite keys and index management. See [the documentation](../../docs/mongodb.md) for the full list.

## Related packages

- [`CodeArchitects.Platform.Data`](../Data) — DAL abstractions
- [`CodeArchitects.Platform.Data.MongoDB.DependencyInjection`](../Data.MongoDB.DependencyInjection) — DI registration

## License

Licensed under the [Apache License 2.0](../../LICENSE).
