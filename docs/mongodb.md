# DAL with MongoDB

> **Note.** The `CodeArchitects.Platform.Data.MongoDB` and
> `CodeArchitects.Platform.Data.MongoDB.DependencyInjection` packages are marked `[Experimental]`:
> breaking changes are very likely to be introduced between one version and the next.
> They have not yet reached the same level of maturity as the other currently supported Data Providers.

MongoDB is a document database: it has no tables or foreign keys, and guarantees atomicity for a
**single document**. The CAEP MongoDB provider exposes the same `IRepository`, `IUnitOfWork`, and
`IDataContext` as the other providers, but some semantics consequently differ.
This page describes those differences.

## Configuration

```csharp
builder.Services.AddData(cfg => cfg
    .UseConnectionString(builder.Configuration.GetConnectionString("Mongo")!)
    .UseDatabase("store")
    .AddEntitiesFrom(typeof(Product).Assembly));
```

`AddData` lives in the `Microsoft.Extensions.DependencyInjection` namespace, as with the other
providers: no additional `using` directive is required in `Program.cs`.

The configuration is **validated immediately**, without contacting the server. A missing client,
missing database, empty model, unresolvable key, or two entities mapped to the same collection
causes startup to fail with an explicit message, rather than a `NullReferenceException` on the
first request.

### Client

```csharp
// from the connection string
.UseConnectionString(connectionString)

// modifying the settings derived from the connection string
.UseConnectionString(connectionString, settings => settings.RetryWrites = true)

// providing the client
.UseClient(client)
.UseClient(sp => sp.GetRequiredService<IMongoClient>())
```

`UseClient` accepts `IMongoClient`, not `MongoClient`: in driver 3.x the concrete class is
`sealed`, so the interface is the only way to decorate or replace the client.

### Options

```csharp
builder.Services.AddData(cfg => cfg
    .UseConnectionString(connectionString)
    .UseDatabase("store")
    .AddEntitiesFrom(typeof(Product).Assembly)
    .AddEntity<LegacyDocument>()                              // explicit registration
    .UseTransactions(TransactionMode.Required)                // default
    .UseGuidRepresentation(GuidRepresentation.Standard)       // default
    .ConfigureConventions(pack => pack.Add(new CamelCaseElementNameConvention()))
    .UseSeed<ApplicationDataSeed>());
```

## Entities and conventions

### Collection name

An entity is a **public, concrete, non-generic** class marked with `[Collection]`:

```csharp
using CodeArchitects.Platform.Data.MongoDB;

[Collection("products")]
public class Product
{
  public Guid Id { get; set; }
  public string? Denomination { get; set; }
  public decimal Price { get; set; }
}
```

`[Table]` is accepted as an alternative for backward compatibility. Without an attribute, the name
is the type name unchanged: there is no pluralization or case transformation. `AddEntity<T>()`
registers a type even if it has no attribute.

> The attribute is called `Collection`, like the xUnit one: in a test project that imports both,
> it must be qualified (`[CodeArchitects.Platform.Data.MongoDB.Collection("...")]`).

### Key

The key is **always** mapped to the `_id` element and is resolved in this order:

1. property marked `[BsonId]`;
2. `Id` property;
3. `<TypeName>Id` property.

Supported types: `Guid`, `string`, `ObjectId`, `int`, `long`. Composite keys are not supported.

Value generation is not handled by the provider: if the key is `default` at insert time, value
generation is delegated to the driver for `ObjectId` and `string`, while `Guid` and integer keys
must be assigned by the domain, as in the CAEP pipeline.

## Aggregates, embedded documents, and references

> **The document is the unit of consistency.** An aggregate root corresponds to a document; the
> entire aggregate lives inside that document. Operations on the aggregate root are therefore
> atomic **by construction**, without transactions.

This changes how associations are modeled compared with relational providers:

| Association | Representation |
|---|---|
| Intra-aggregate (1:1 and 1:N) | embedded sub-document or array of **embedded** sub-documents |
| Inter-aggregate (1:1, N:1, 1:N) | field containing the other aggregate's **key** |
| Many-to-many | array of keys on the owning side; no junction collection |

```csharp
[Collection("carts")]
public class Cart
{
  public Guid Id { get; set; }
  public List<CartItem> Items { get; set; } = [];   // intra-aggregate: embedded
  public Guid CustomerId { get; set; }              // inter-aggregate: reference
}

public class CartItem          // no [Collection], no Id: not an entity
{
  public string? Sku { get; set; }
  public int Quantity { get; set; }
}
```

Embedded entities **do not have their own collection** and cannot be reached through a dedicated
repository: an entity that needs a repository is, by definition, an aggregate root.

Writes follow the semantics already documented in the [DAL](dataaccesslayer.md#associazioni-e-aggregati):
`Insert` and `Update` on the aggregate root write the entire document, including embedded entities,
while inter-aggregate entities are not written, only the reference is persisted. `Remove` deletes
the document and its embedded entities; there is **no** cascade through references, because MongoDB
has no `delete behavior`: this remains the application's responsibility.

`DBRef` is intentionally not supported: it cannot be resolved server-side in a `$lookup` pipeline
and puts the collection name into the data.

## Unit of work and transactions

The use of `IUnitOfWorkManager` and `IUnitOfWork` is identical to the other providers
([DAL](dataaccesslayer.md#il-pattern-unit-of-work)). There are two differences.

**Writes are deferred.** Within the UnitOfWork, operations accumulate and are applied on
`SaveAsync` (or on `Dispose` with `autoSave: true`), all in **a single** MongoDB transaction.
Consequently, a read performed within the UnitOfWork context **does not see** writes that have not
yet been committed.

```csharp
await using (IUnitOfWork uow = _uowManager.Begin())
{
  await _cartRepo.UpdateAsync(cart);
  await _productRepo.UpdateAsync(product);

  await uow.SaveAsync();   // one transaction: both or neither
}
```

**Transactions require a replica set.** MongoDB does not support them on a standalone server. The
behavior is governed by `UseTransactions`:

| Mode | Behavior on an incompatible topology |
|---|---|
| `Required` (default) | throws `TransactionsNotSupportedException` |
| `WhenSupported` | runs without atomicity and emits a warning to the logger |
| `Disabled` | never uses transactions |

A write to a single document **outside** a UnitOfWork does not open a transaction: it is already
atomic. `InsertMany` and `UpdateMany`, however, always open one because they involve multiple
documents.

## Operation semantics

Every row applies to both the synchronous and the asynchronous variant: they run the same driver
operation, on the same session, with the same success criterion and the same exceptions.

| Method | Behavior | Exception |
|---|---|---|
| `Find(key)` | filter on `_id` | — (`null` if absent) |
| `Find(key, include)` | as `Find(key)`: embedded navigations are already loaded, see below | `NotSupportedException` for references to other collections |
| `Insert` | `insertOne` | `MongoWriteException` on duplicate key |
| `InsertMany` | ordered `insertMany`, in a transaction | `MongoBulkWriteException` on duplicate key |
| `Update` | replaces the entire document | `DBConcurrencyException` if it does not exist |
| `UpdateMany` | ordered `bulkWrite` of replacements, in a transaction | `DBConcurrencyException` if any do not exist |
| `Upsert` | replaces or inserts | `DBConcurrencyException` if not applied |
| `Remove(entity)` / `Remove(key)` | `deleteOne` | `DBConcurrencyException` if it does not exist |

Some points may surprise users coming from relational providers:

- **`Update` replaces the entire document**, not only the modified fields: there is no change
  tracking. For a partial update, use `Collection.UpdateOneAsync` directly with the current session.
- **An update or upsert that changes nothing is successful.** The criterion is "the document
  exists", not "the document was rewritten".
- **`InsertMany` and `UpdateMany` with an empty sequence do nothing**: no round-trip and no
  transaction, so they succeed on a standalone server too. A `null` element in the sequence throws
  `ArgumentException`.
- **Duplicate keys** surface as the driver's own `MongoWriteException` / `MongoBulkWriteException`,
  unchanged, as the ADO.NET provider does with the database exceptions. In a transaction, nothing
  of the failed commit is persisted.
- **Unacknowledged writes** (write concern `w: 0`) are not checked: the server returns no counts,
  so a missing document cannot be told apart from a successful write, and no
  `DBConcurrencyException` is raised.
- **Cancellation.** Outside a unit of work the token reaches the driver. Inside a unit of work an
  operation is only queued: its token is checked when it is queued (an operation already cancelled
  is not queued and throws `OperationCanceledException`), while the commit is governed by the token
  passed to `SaveAsync`.

### Include

The provider behaves as EF Core does with owned types. **Embedded navigations are always loaded**
with the document, so including them is accepted and has no effect. This keeps provider-agnostic
code, such as generated code that calls `Include`, working unchanged on MongoDB.

```csharp
// all valid, and equivalent to FindAsync(id): Items and their children are in the document
await cartRepository.FindAsync(id, include => include.Include(cart => cart.Items));
await cartRepository.FindAsync(id, include => include
  .Include(cart => cart.Items, items => items.Include(item => item.Discount)));
await cartRepository.FindAsync(id, include => include.Include("Items.Discount"));
```

Every navigation in the path is validated **before** the query, so the outcome does not depend
on whether the document exists. A request the provider cannot satisfy is never ignored:

| Request | Exception |
|---|---|
| navigation to an entity stored in **another collection** (a type with `[Collection]` / `[Table]`) | `NotSupportedException` |
| member that is **not persisted** (for example `[BsonIgnore]`) | `NotSupportedException` |
| member that does not exist, a scalar value, or an expression that is not a member access (a filtered include such as `x => x.Items.Where(...)`) | `InvalidOperationException` |

References between collections are only keys, and the provider does not resolve them, as the
EF Core providers for document databases also do. Load the referenced aggregate from its own
repository, or query its collection from a specialized repository, passing `Session`.

## Repository

### Direct repository

The domain entity is the document. Use this when the domain model is serializable 1:1.

```csharp
using CodeArchitects.Platform.Data.MongoDB;

public class ProductRepository : MongoDBRepository<Product, Guid>, IProductRepository
{
  public ProductRepository(IDataContext context) : base(context) { }

  public async Task<IEnumerable<Product>> GetTopSellingProductsAsync(int count, CancellationToken ct = default)
  {
    return await Collection
      .Find(Session, Builders<Product>.Filter.Empty)   // Session: participates in the unit of work
      .SortByDescending(product => product.SaleCount)
      .Limit(count)
      .ToListAsync(ct);
  }
}
```

The base class exposes `Collection` (`IMongoCollection<TEntity>`), `Database`, and **`Session`**.

> Passing `Session` to custom queries is not optional: an operation executed without a session runs
> on an implicit session, and therefore **outside** the transaction of the current unit of work.

The repository is registered and consumed through the CAEP abstractions, like the repositories of
the other providers: `AddData` registers the MongoDB `IDataContext` the constructor needs.

```csharp
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// a plain repository, without specialized queries
builder.Services.AddScoped<IRepository<Category, Guid>, MongoDBRepository<Category, Guid>>();
```

### Mapped repository

When the document must differ from the domain, for example for denormalization, technical fields,
different names, or schema versioning, use `MongoDBMappedRepository<TDocument, TEntity, TKey>`.
`[Collection]` belongs on the **document**, never on the domain entity.

```csharp
[Collection("products")]
public class ProductDocument
{
  public Guid Id { get; set; }
  public string? Code { get; set; }
  public decimal Amount { get; set; }
}

public class ProductRepository : MongoDBMappedRepository<ProductDocument, Product, Guid>, IProductRepository
{
  public ProductRepository(IDataContext context) : base(context) { }

  protected override Product TableToEntity(ProductDocument document) => /* ... */;
  protected override ProductDocument EntityToTable(Product entity) => /* ... */;
}
```

The base class exposes `Collection` and its `Documents` alias, both of type
`IMongoCollection<TDocument>`.

Mapping can be delegated to Mapster with a bidirectional `TypeAdapterConfig`, as described in
[DAL – Mapped repository](dataaccesslayer.md#mapped-repository):

```csharp
config.NewConfig<Product, ProductDocument>().TwoWays();
```

> The `PreserveTracking` extension from `CodeArchitects.Platform.Data.Mapster` has **no effect** on
> this provider: MongoDB has no change tracking.

## Seeding

Seeding uses the same `DataSeed` as the other providers.

```csharp
[SeedOrder(1)]
public class CategorySeed : DataSeed
{
  public override void Seed(ISeeder seeder) => seeder.Seed(
    new Category { ... },
    new Category { ... });
}
```

Registration and application:

```csharp
builder.Services.AddData(cfg => cfg
    // ...
    .UseSeed<CategorySeed>()
    .AddSeedsFrom(typeof(CategorySeed).Assembly));   // or by scanning

WebApplication app = builder.Build();
app.Services.SeedMongo();                             // or await SeedMongoAsync()
```

- The **order** is deterministic: `[SeedOrder]` (missing ⇒ `0`), then the type name for equal order.
- Seeding is **idempotent per collection**: a seed is applied only to an empty collection.
- All seeds are committed **in a single transaction**: interrupted seeding does not leave the
  database partially populated. It therefore requires a replica set.

## Testing

The provider's integration tests use [Testcontainers](https://dotnet.testcontainers.org/) with a
single-node replica set, which is required for transactions:

```csharp
MongoDbContainer container = new MongoDbBuilder()
  .WithImage("mongo:7.0")
  .WithReplicaSet()
  .Build();
```

## Known limitations

Not yet supported, in order of impact:

- **`Include` of references between collections** and **filtered includes** — see
  [Include](#include). Embedded navigations are supported.
- **Optimistic concurrency** — no version token.
- **Multitenancy and soft delete** — available in the EF Core provider, not here.
- **Change tracking** — has no equivalent in the aggregate-based model.
- **Composite keys**, **index management**, **multiple databases** with a typed context.
- **Interceptors** for operations.

## Packages

- [`CodeArchitects.Platform.Data.MongoDB`](https://www.nuget.org/packages/CodeArchitects.Platform.Data.MongoDB)
- [`CodeArchitects.Platform.Data.MongoDB.DependencyInjection`](https://www.nuget.org/packages/CodeArchitects.Platform.Data.MongoDB.DependencyInjection)

They require **MongoDB Server 4.4 or later** (a requirement of driver 3.x).
