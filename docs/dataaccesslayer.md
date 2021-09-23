# Data access layer

## The repository pattern

Repositories are classes or components that encapsulate the logic required to access data sources. They centralize common data access functionality, providing better maintainability and decoupling the infrastructure or technology used to access databases from the domain model layer.

The CodeArchitects.Platform.Data package provides the interfaces that define the repository pattern. Let's have a look at the `IRepository` interface.

First, we demand all entities being accessed via the repository to implement the `IEntity<TKey>` interface.

```c#
public interface IEntity
{
    object Id { get; }
}

public interface IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    new TKey Id { get; }
}
```

The repository interface is the following.

```c#
public interface IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    IQueryable<TEntity> Query();
    TEntity? Find(TKey id);
    ValueTask<TEntity?> FindAsync(TKey id, CancellationToken    cancellationToken = default);
    void Update(TEntity entity);
    void Add(TEntity entity);
    void Delete(TEntity entity);
}
```

The current implementation of this interface uses Entity Framework Core. You can find the concrete `Repository<TEntity, TKey>` class inside the CodeArchitects.Platform.Data.EntityFrameworkCore package.

## The generic repository pattern

The pattern we advice to implement is the generic repository pattern. To implement it, define a specialized repository interface for your entities, which will inherit from the generic interface. This specialized repository will define specific queries for its entity. Then do the same for the concrete implementation.

### Example

Suppose we have a `Product` entity.

```c#
public class Product : IEntity<Guid>
{
    public Guid Id { get; set; }
    public decimal Price { get; set; }
    public int SaleCount { get; set; }
}
```

Its repository will expose a method to retrieve the top 10 selling products.

```c#
public interface IProductRepository : IRepository<Product, Guid>
{
    Task<IEnumerable<Product>> GetTopSellingProductsAsync();
}
```

Then we can implement the specic concrete repository and have the basic methods already implemented.

```c#
public class ProductRepository : Repository<Product, Guid>, IProductRepository
{
    public Task<IEnumerable<Product>> GetTopSellingProductsAsync()
    {
        return Query()
            .OrderBy(x => x.SaleCount)
            .Take(10)
            .ToList();
    }
}
```
