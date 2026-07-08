# CodeArchitects.Platform.Data.EntityFrameworkCore

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Data.EntityFrameworkCore.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Data.EntityFrameworkCore)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

An **Entity Framework Core** implementation of the [`CodeArchitects.Platform.Data`](../Data) Data
Access Layer. It provides repository base classes and value-added behaviors (soft delete,
multitenancy, interceptors) on top of EF Core.

> Requires .NET 6 or later. For .NET Core 3.1 / .NET 5, use
> [`CodeArchitects.Platform.Data.EntityFrameworkCore5`](../Data.EntityFrameworkCore5).

## Installation

```bash
dotnet add package CodeArchitects.Platform.Data.EntityFrameworkCore
```

## Implementing a repository

Derive from `EFCoreRepository<TEntity, TKey>`; the protected `Entities` property exposes the
`DbSet<TEntity>`:

```csharp
using CodeArchitects.Platform.Data.EntityFrameworkCore;

public class ProductRepository : EFCoreRepository<Product, Guid>, IProductRepository
{
    public ProductRepository(IDataContext context) : base(context) { }

    public async Task<IEnumerable<Product>> GetTopSellingProductsAsync(int count, CancellationToken ct = default) =>
        await Entities.OrderBy(x => x.SaleCount).Take(count).ToListAsync(ct);
}
```

For a custom database↔domain mapping, derive from `EFCoreMappedRepository<TTable, TEntity, TKey>` and
implement `EntityToTable`/`TableToEntity`.

## Value-added behaviors

| Feature | Type |
|---------|------|
| Soft delete | `ISoftDeleteDescriptor`, `DefaultSoftDeleteDescriptor` |
| Multitenancy | `IMultitenancyDescriptor`, `DefaultMultitenancyDescriptor` |
| Modification interception | `IModificationInterceptor` (`AddModificationInterceptor`) |
| Query-root rewriting | `IQueryRootExpressionInterceptor` (`AddQueryRootExpressionInterceptor`) |

## Related packages

- [`CodeArchitects.Platform.Data`](../Data) — DAL abstractions
- [`CodeArchitects.Platform.Data.EntityFrameworkCore.DependencyInjection`](../Data.EntityFrameworkCore.DependencyInjection) — DI registration

## License

Licensed under the [Apache License 2.0](../../LICENSE).
