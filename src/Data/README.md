# CodeArchitects.Platform.Data

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Data.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Data)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

The technology-agnostic **Data Access Layer (DAL)** abstractions for the Code Architects Platform.
It defines the Repository and Unit of Work patterns independently of any ORM or database provider.
Pick an implementation — [Entity Framework Core](../Data.EntityFrameworkCore),
[ADO.NET](../Data.AdoNet) or [MongoDB](../Data.MongoDB) — to provide the concrete behavior.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Data
```

## Repository pattern

The generic `IRepository<TEntity, TKey>` models CRUD access and is meant to be extended per entity:

```csharp
public interface IRepository<TEntity, TKey>
    where TEntity : class
    where TKey : IEquatable<TKey>
{
    Task<TEntity?> FindAsync(TKey key, CancellationToken cancellationToken = default);
    Task<TEntity?> FindAsync(TKey key, IncludeAction<TEntity> includeAction, CancellationToken cancellationToken = default);
    Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task UpsertAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task RemoveAsync(TKey key, CancellationToken cancellationToken = default);
}
```

Define a specialized repository for your aggregate:

```csharp
public interface IProductRepository : IRepository<Product, Guid>
{
    Task<IEnumerable<Product>> GetTopSellingProductsAsync(int count, CancellationToken cancellationToken = default);
}
```

## Unit of Work

Group multiple writes into a single atomic operation:

```csharp
await using IUnitOfWork uow = _uowManager.Begin(autoSave: true, cancellationToken);
await _cartRepo.SetStatusToCompletedAsync(cart.Id, cancellationToken);
foreach (Product product in cart.Products)
    await _productRepo.IncrementSaleCountAsync(product.Id);
// committed automatically on scope exit when autoSave is true
```

`IUnitOfWork` can also be injected directly (registered as `Scoped`).

## Also provides

- `IDataContext` — generic, entity-typed CRUD used by repository base classes.
- Aggregate associations (intra-/inter-aggregate reads and writes).
- `DataSeed` / `ISeeder` — database seeding.

## Implementations

- [`CodeArchitects.Platform.Data.EntityFrameworkCore`](../Data.EntityFrameworkCore)
- [`CodeArchitects.Platform.Data.AdoNet`](../Data.AdoNet)
- [`CodeArchitects.Platform.Data.MongoDB`](../Data.MongoDB)

## License

Licensed under the [Apache License 2.0](../../LICENSE).
