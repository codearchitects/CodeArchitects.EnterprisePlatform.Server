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

## Implementing a repository

Derive from `MongoDBRepository<TEntity, TKey>`:

```csharp
public class ProductRepository : MongoDBRepository<Product, Guid>, IProductRepository
{
    public ProductRepository(IDataContext context) : base(context) { }
}
```

For a custom document↔domain mapping, derive from `MongoDBMappedRepository<TDocument, TEntity, TKey>`.

## Related packages

- [`CodeArchitects.Platform.Data`](../Data) — DAL abstractions
- [`CodeArchitects.Platform.Data.MongoDB.DependencyInjection`](../Data.MongoDB.DependencyInjection) — DI registration

## License

Licensed under the [Apache License 2.0](../../LICENSE).
