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
different names, value objects, or schema versioning, use a mapped repository. `[Collection]` belongs
on the **document**, never on the domain entity: only the document is registered in the model, and
`Collection` / `Documents` are typed `IMongoCollection<TDocument>`.

| Base class | Package | Mapping |
|---|---|---|
| `MongoDBMappedRepository<TDocument, TEntity, TKey>` | `CodeArchitects.Platform.Data.MongoDB` | written by hand, overriding `TableToEntity` and `EntityToTable` |
| `MongoDBMapsterRepository<TDocument, TEntity, TKey>` | `CodeArchitects.Platform.Data.MongoDB.Mapster` | Mapster, with rules registered through `AddDataMapster` |

The direct repository and the hand-written mapped repository do not depend on Mapster: only the
`.Mapster` package brings it in.

#### With Mapster

```bash
dotnet add package CodeArchitects.Platform.Data.MongoDB.Mapster
```

```csharp
builder.Services
  .AddData(cfg => cfg
    .UseConnectionString(connectionString)
    .UseDatabase("store")
    .AddEntitiesFrom(typeof(PurchaseOrderDocument).Assembly))
  .AddDataMapster(typeof(PurchaseOrderMapping).Assembly)   // or AddDataMapster(config => ...)
  // a plain repository...
  .AddScoped<IRepository<Ticket, string>, MongoDBMapsterRepository<TicketDocument, Ticket, string>>()
  // ...or a specialized one, derived from MongoDBMapsterRepository
  .AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
```

`AddDataMapster` builds a dedicated `TypeAdapterConfig`, never `TypeAdapterConfig.GlobalSettings`,
and registers a scoped `IDataMapper` on it, leaving the application's own `IMapper` and `TypeAdapterConfig`
alone. It is called once, with every rule of the data layer. The configuration is **strict**, and every
rule is **compiled at registration**,
so a mapping error makes startup fail with an explicit `InvalidOperationException` instead of surfacing
on the first request:

- every pair of types needs a rule, nested types included (`RequireExplicitMapping`);
- every destination member needs a source or an explicit `Ignore` (`RequireDestinationMemberSource`).
  This also applies to the rules reversed by `TwoWays()`, where Mapster would otherwise skip the check.
  Only the members configured **after** `TwoWays()` are reversed;
- the repository constructor checks that both directions, `TEntity → TDocument` and
  `TDocument → TEntity`, have a rule.

Both settings can be relaxed inside `AddDataMapster(config => ...)`. The repository then compiles the
implicit mapping of its two types when it is created, so an invalid one still fails early.

#### Example

The domain aggregate uses value objects, an embedded collection and a reference to another aggregate.
The document renames and flattens fields and adds a technical one.

```csharp
public sealed record PurchaseOrderNumber(string Value);
public sealed record Money(decimal Amount, string Currency);
public sealed record Address(string Street, string City, string PostalCode, string Country);
public sealed record OrderLine(Guid ProductId, int Quantity, Money UnitPrice);

public class PurchaseOrder                       // domain: no [Collection]
{
  public Guid Id { get; set; }
  public PurchaseOrderNumber Number { get; set; }
  public Money Total { get; set; }
  public Address ShippingAddress { get; set; }
  public List<OrderLine> Lines { get; set; } = [];
  public Guid CustomerId { get; set; }           // reference to another aggregate: its key only
}

[Collection("purchase_orders")]
public class PurchaseOrderDocument               // persistence
{
  public Guid Id { get; set; }
  public string Code { get; set; }               // PurchaseOrderNumber
  public decimal TotalAmount { get; set; }       // Money, flattened
  public string TotalCurrency { get; set; }
  public AddressDocument Shipping { get; set; }  // embedded sub-document
  public List<OrderLineDocument> Lines { get; set; } = [];
  public Guid CustomerId { get; set; }
  public int SchemaVersion { get; set; }         // technical: written, never read into the domain
}
```

```csharp
public sealed class PurchaseOrderMapping : IRegister
{
  public void Register(TypeAdapterConfig config)
  {
    // single-value value object <-> scalar
    config.NewConfig<PurchaseOrderNumber, string>().MapWith(number => number.Value);
    config.NewConfig<string, PurchaseOrderNumber>().MapWith(value => new PurchaseOrderNumber(value));

    // immutable value object: shared as it is instead of copied
    config.NewConfig<Money, Money>().MapWith(money => money);

    config.NewConfig<PurchaseOrder, PurchaseOrderDocument>()
      .Map(document => document.Code, order => order.Number)
      .Map(document => document.TotalAmount, order => order.Total.Amount)
      .Map(document => document.TotalCurrency, order => order.Total.Currency)
      .Map(document => document.Shipping, order => order.ShippingAddress)
      .Map(document => document.SchemaVersion, _ => 2);

    config.NewConfig<PurchaseOrderDocument, PurchaseOrder>()
      .Map(order => order.Number, document => document.Code)
      .Map(order => order.Total, document => new Money(document.TotalAmount, document.TotalCurrency))
      .Map(order => order.ShippingAddress, document => document.Shipping);

    // one rule per nested pair, in both directions
    config.NewConfig<Address, AddressDocument>()
      .Map(document => document.Line1, address => address.Street)
      .Map(document => document.Zip, address => address.PostalCode)
      .Map(document => document.CountryCode, address => address.Country);
    config.NewConfig<AddressDocument, Address>()
      .MapWith(document => new Address(document.Line1, document.City, document.Zip, document.CountryCode));
    // ... OrderLine <-> OrderLineDocument likewise

    // same shape except for one member: one rule, reversed by TwoWays
    config.NewConfig<Ticket, TicketDocument>()
      .TwoWays()
      .Map(document => document.Subject, ticket => ticket.Title);
  }
}
```

The complete example, with its tests against a real server, is in
`tests/Data.MongoDB.IntegrationTests` (`Fixtures/PurchaseOrders.cs` and `MapsterRepositoryTests.cs`).

#### Behavior

**Operations.** Every operation maps between the two models and then runs the same `IDataContext`
operation as the direct repository, on the document. Transactions, deferral inside a unit of work,
`DBConcurrencyException`, empty batches, and cancellation follow [Operation semantics](#operation-semantics)
and [Unit of work and transactions](#unit-of-work-and-transactions) unchanged.

**Checked mapping.** A `null` entity throws `ArgumentNullException`, a `null` element of a batch
throws `ArgumentException` with its index, and a mapping that returns `null` throws
`InvalidOperationException`. `InsertMany` and `UpdateMany` map the whole batch **before** writing or
queuing anything: an element that fails to map is named by its index, and nothing of the batch reaches
the database or the unit of work.

**Keys.** The document key follows the usual [rules](#key). On the domain entity the repository looks
for `Id`, then `<TypeName>Id`, of the repository key type:

- on every write, if the entity has a key and the mapped document has a different one,
  `InvalidOperationException` is thrown: the mapping lost or changed the identity;
- on `Insert` and `InsertMany`, an empty document key is generated as the driver would (for example an
  `ObjectId`, also for a `string` key marked `[BsonRepresentation(BsonType.ObjectId)]`) and written back
  to the entity, even through a private setter. The entity has its identity as soon as the operation is
  issued, also inside a unit of work where the write is deferred to the commit.

If the domain entity has no key the convention can find, these checks are skipped.

**Aggregates, embedded documents, and references.** An aggregate maps to one document: its parts map
to embedded sub-documents and arrays, with one rule per nested pair. A reference to another aggregate
is persisted as its key. If the domain holds the other aggregate as an object, map its key on the way
out and `Ignore` the object on the way back, then load it from its own repository: strict mode forces
that choice to be written down.

**Queries and filters.** Custom queries are written on the **document**, with `Documents`, `Session`
and `Builders<TDocument>`, and their results are mapped with `ToEntities` / `ToEntitiesAsync`:

```csharp
public class PurchaseOrderRepository(IDataContext context, IDataMapper mapper)
  : MongoDBMapsterRepository<PurchaseOrderDocument, PurchaseOrder, Guid>(context, mapper), IPurchaseOrderRepository
{
  public Task<List<PurchaseOrder>> FindByCustomerAsync(Guid customerId, CancellationToken ct = default) =>
    ToEntitiesAsync(
      Documents.Find(Session, document => document.CustomerId == customerId).SortBy(document => document.Code),
      ct);
}
```

A predicate on the domain entity cannot be translated to the document: Mapster maps objects, not
expressions, and a renamed or flattened member has no field to compare with. The repository therefore
offers no API that accepts one. An untranslatable query fails to compile instead of returning wrong
results at runtime. `ToEntity`, `ToDocument` and `ToEntities(IEnumerable<TDocument>)` are also
available to derived repositories.

**Include.** `Find(key, include)` throws `NotSupportedException`, as in the mapped repositories of
the other providers. The include is written on the domain entity, which is not part of the model, so
it cannot be validated against the document. It is not needed either: the document is loaded whole, so
`Find(key)` already returns the embedded parts of the aggregate.

#### Hand-written mapping

Without Mapster, derive from `MongoDBMappedRepository` and write the two mappings. Every behavior above
applies in the same way.

```csharp
public class ProductRepository : MongoDBMappedRepository<ProductDocument, Product, Guid>, IProductRepository
{
  public ProductRepository(IDataContext context) : base(context) { }

  protected override Product TableToEntity(ProductDocument document) => /* ... */;
  protected override ProductDocument EntityToTable(Product entity) => /* ... */;
}
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
- [`CodeArchitects.Platform.Data.MongoDB.Mapster`](https://www.nuget.org/packages/CodeArchitects.Platform.Data.MongoDB.Mapster), optional: mapped repository based on Mapster

They require **MongoDB Server 4.4 or later** (a requirement of driver 3.x).
