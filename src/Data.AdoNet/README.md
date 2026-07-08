# CodeArchitects.Platform.Data.AdoNet

[![NuGet](https://img.shields.io/nuget/v/CodeArchitects.Platform.Data.AdoNet.svg)](https://www.nuget.org/packages/CodeArchitects.Platform.Data.AdoNet)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](../../LICENSE)

An **ADO.NET** implementation of the [`CodeArchitects.Platform.Data`](../Data) Data Access Layer,
typically used together with [Dapper](https://github.com/DapperLib/Dapper). Choose this over the
Entity Framework Core implementation when you want full control over the SQL that is executed.

## Installation

```bash
dotnet add package CodeArchitects.Platform.Data.AdoNet
```

Add the provider package for your database (see [Related packages](#related-packages)).

## Implementing a repository

Derive from `AdoNetRepository<TEntity, TKey>` and use the protected `Connection` property to run
queries:

```csharp
using CodeArchitects.Platform.Data.AdoNet;

public class ProductRepository : AdoNetRepository<Product, Guid>, IProductRepository
{
    public ProductRepository(IDataContext context) : base(context) { }

    public async Task<IEnumerable<Product>> GetTopSellingProductsAsync(int count, CancellationToken ct = default)
    {
        const string query = "SELECT [Id], [Price], [SaleCount] FROM [Products] ORDER BY [SaleCount] LIMIT @count";
        return await Connection.QueryAsync<Product>(query, new { count });
    }
}
```

For full control over the mapping between database and domain models, derive from
`AdoNetMappedRepository<TTable, TEntity, TKey>` and implement `EntityToTable`/`TableToEntity`.

## Related packages

- [`CodeArchitects.Platform.Data`](../Data) — DAL abstractions
- [`CodeArchitects.Platform.Data.AdoNet.DependencyInjection`](../Data.AdoNet.DependencyInjection) — DI registration
- Providers: [SQL Server](../Data.AdoNet.SQLServer) · [PostgreSQL](../Data.AdoNet.PostgreSQL) · [MySQL](../Data.AdoNet.MySQL) · [Oracle](../Data.AdoNet.Oracle)

## License

Licensed under the [Apache License 2.0](../../LICENSE).
