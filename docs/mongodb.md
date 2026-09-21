# DAL con MongoDB

> **Nota.** I pacchetti `CodeArchitects.Platform.Data.MongoDB` e
> `CodeArchitects.Platform.Data.MongoDB.DependencyInjection` sono marcati `[Experimental]`: la
> superficie pubblica può cambiare fra due versioni minori. Le funzionalità non ancora supportate
> sono elencate in [Limiti noti](#limiti-noti); l'architettura di riferimento completa è in
> [`docs/design/mongodb-provider.md`](design/mongodb-provider.md).

MongoDB è un database documentale: non ha tabelle né foreign key, e garantisce atomicità sul
**singolo documento**. Il provider MongoDB del DAL CAEP espone gli stessi `IRepository`,
`IUnitOfWork` e `IDataContext` degli altri provider, ma alcune semantiche cambiano di conseguenza.
Questa pagina descrive quelle differenze.

## Configurazione

```csharp
builder.Services.AddData(cfg => cfg
    .UseConnectionString(builder.Configuration.GetConnectionString("Mongo")!)
    .UseDatabase("store")
    .AddEntitiesFrom(typeof(Product).Assembly));
```

`AddData` vive nel namespace `Microsoft.Extensions.DependencyInjection`, come per gli altri
provider: in un `Program.cs` non serve alcun `using` aggiuntivo.

La configurazione è **validata subito**, senza contattare il server. Client mancante, database
mancante, modello vuoto, chiave non risolvibile o due entità mappate sulla stessa collection
fanno fallire l'avvio con un messaggio esplicito, non una `NullReferenceException` alla prima
richiesta.

### Client

```csharp
// dalla connection string
.UseConnectionString(connectionString)

// intervenendo sulle impostazioni derivate dalla connection string
.UseConnectionString(connectionString, settings => settings.RetryWrites = true)

// fornendo il client
.UseClient(client)
.UseClient(sp => sp.GetRequiredService<IMongoClient>())
```

`UseClient` accetta `IMongoClient`, non `MongoClient`: nel driver 3.x la classe concreta è
`sealed`, quindi l'interfaccia è l'unico modo per decorare o sostituire il client.

### Opzioni

```csharp
builder.Services.AddData(cfg => cfg
    .UseConnectionString(connectionString)
    .UseDatabase("store")
    .AddEntitiesFrom(typeof(Product).Assembly)
    .AddEntity<LegacyDocument>()                              // registrazione esplicita
    .UseTransactions(TransactionMode.Required)                // default
    .UseGuidRepresentation(GuidRepresentation.Standard)       // default
    .ConfigureConventions(pack => pack.Add(new CamelCaseElementNameConvention()))
    .UseSeed<ApplicationDataSeed>());
```

## Entità e convenzioni

### Nome della collection

Un'entità è una classe **pubblica, concreta e non generica** marcata con `[Collection]`:

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

`[Table]` è accettato come alternativa, per retro-compatibilità. Senza attributo il nome è quello
del tipo, invariato: nessuna pluralizzazione e nessuna trasformazione di case. `AddEntity<T>()`
registra un tipo anche se non ha attributo.

> L'attributo si chiama `Collection` come quello di xUnit: in un progetto di test che importa
> entrambi va qualificato (`[CodeArchitects.Platform.Data.MongoDB.Collection("...")]`).

### Chiave

La chiave è **sempre** mappata sull'elemento `_id` ed è risolta in quest'ordine:

1. proprietà marcata `[BsonId]`;
2. proprietà `Id`;
3. proprietà `<NomeTipo>Id`.

Tipi supportati: `Guid`, `string`, `ObjectId`, `int`, `long`. Le chiavi composite non sono
supportate.

La generazione del valore non è a carico del provider: se la chiave è `default` al momento
dell'insert, il valore viene delegato al driver per `ObjectId` e `string`, mentre per `Guid` e per
gli interi va valorizzata dal dominio — com'è nella pipeline CAEP.

## Aggregati, documenti incorporati e riferimenti

> **L'unità di consistenza è il documento.** Un aggregate root corrisponde a un documento; tutto
> l'aggregato vive dentro quel documento. Le operazioni sull'aggregate root sono quindi atomiche
> **per costruzione**, senza transazioni.

Questo cambia il modo di modellare le associazioni rispetto ai provider relazionali:

| Associazione | Rappresentazione |
|---|---|
| Intra-aggregato (1:1 e 1:N) | sotto-documento o array di sotto-documenti **incorporati** |
| Inter-aggregato (1:1, N:1, 1:N) | campo contenente la **chiave** dell'altro aggregato |
| Molti-a-molti | array di chiavi sul lato proprietario; nessuna collection di giunzione |

```csharp
[Collection("carts")]
public class Cart
{
  public Guid Id { get; set; }
  public List<CartItem> Items { get; set; } = [];   // intra-aggregato: incorporato
  public Guid CustomerId { get; set; }              // inter-aggregato: riferimento
}

public class CartItem          // nessun [Collection], nessun Id: non è un'entità
{
  public string? Sku { get; set; }
  public int Quantity { get; set; }
}
```

Le entità incorporate **non hanno una collection propria** e non sono raggiungibili da un
repository dedicato: un'entità che deve avere un repository è per definizione un aggregate root.

Le scritture seguono la semantica già documentata nel [DAL](dataaccesslayer.md#associazioni-e-aggregati):
`Insert` e `Update` sull'aggregate root scrivono l'intero documento, incorporati compresi, mentre
le entità inter-aggregato non vengono scritte — viene persistito solo il riferimento. `Remove`
cancella il documento e con esso gli incorporati; **non** esiste cascata attraverso i riferimenti,
perché MongoDB non ha un `delete behavior`: resta responsabilità applicativa.

`DBRef` non è supportato di proposito: non è risolvibile lato server in una pipeline `$lookup` e
mette il nome della collection dentro il dato.

## Unit of work e transazioni

L'uso di `IUnitOfWorkManager` e `IUnitOfWork` è identico agli altri provider
([DAL](dataaccesslayer.md#il-pattern-unit-of-work)). Le differenze sono due.

**Le scritture sono differite.** Dentro un'unità di lavoro le operazioni si accumulano e vengono
applicate al `SaveAsync` (o al `Dispose` con `autoSave: true`), tutte in **un'unica transazione**
MongoDB. Ne consegue che una lettura effettuata dentro l'unità di lavoro **non vede** le scritture
ancora non committate.

```csharp
await using (IUnitOfWork uow = _uowManager.Begin())
{
  await _cartRepo.UpdateAsync(cart);
  await _productRepo.UpdateAsync(product);

  await uow.SaveAsync();   // una sola transazione: o entrambe, o nessuna
}
```

**Le transazioni richiedono un replica set.** MongoDB non le supporta su un server standalone. Il
comportamento è governato da `UseTransactions`:

| Modalità | Comportamento su topologia non compatibile |
|---|---|
| `Required` (default) | solleva `TransactionsNotSupportedException` |
| `WhenSupported` | esegue senza atomicità ed emette un warning sul logger |
| `Disabled` | non usa mai transazioni |

Il default è `Required` perché l'unità di lavoro è una **promessa di atomicità**: eseguirla senza
garanzie, in silenzio, è peggio di un errore esplicito. Per lo sviluppo locale su istanza
standalone si usa `Disabled`; il `docker-compose` di un servizio che usa l'unità di lavoro dovrebbe
però prevedere un replica set a nodo singolo.

Una scrittura su un singolo documento **fuori** da un'unità di lavoro non apre alcuna transazione:
è già atomica. `InsertMany` e `UpdateMany` invece la aprono sempre, perché coinvolgono più
documenti.

## Semantica delle operazioni

| Metodo | Comportamento | Eccezione |
|---|---|---|
| `FindAsync(key)` | filtro su `_id` | — (`null` se assente) |
| `FindAsync(key, include)` | **non supportato** | `NotSupportedException` |
| `InsertAsync` | `insertOne` | `MongoWriteException` su chiave duplicata |
| `InsertManyAsync` | `insertMany` ordinata, in transazione | `MongoBulkWriteException` |
| `UpdateAsync` | sostituisce l'intero documento | `DBConcurrencyException` se non esiste |
| `UpdateManyAsync` | `bulkWrite` ordinata, in transazione | `DBConcurrencyException` se qualcuno non esiste |
| `UpsertAsync` | sostituisce o inserisce | `DBConcurrencyException` se non applicato |
| `RemoveAsync` | `deleteOne` | `DBConcurrencyException` se non esiste |

Due punti che sorprendono chi arriva dai provider relazionali:

- **`UpdateAsync` sostituisce l'intero documento**, non i soli campi modificati: non esiste change
  tracking. Per un aggiornamento parziale si usa direttamente `Collection.UpdateOneAsync` con la
  sessione corrente.
- **Un aggiornamento che non cambia nulla è un successo.** Il criterio è «il documento esiste», non
  «il documento è stato riscritto».

## Repository

### Repository diretto

L'entità di dominio **è** il documento. Da usare quando il modello di dominio è serializzabile 1:1.

```csharp
using CodeArchitects.Platform.Data.MongoDB;

public class ProductRepository : MongoDBRepository<Product, Guid>, IProductRepository
{
  public ProductRepository(IDataContext context) : base(context) { }

  public async Task<IEnumerable<Product>> GetTopSellingProductsAsync(int count, CancellationToken ct = default)
  {
    return await Collection
      .Find(Session, Builders<Product>.Filter.Empty)   // Session: partecipa all'unità di lavoro
      .SortByDescending(product => product.SaleCount)
      .Limit(count)
      .ToListAsync(ct);
  }
}
```

La classe base espone `Collection` (`IMongoCollection<TEntity>`), `Database` e **`Session`**.

> Passare `Session` alle query personalizzate non è opzionale: un'operazione eseguita senza sessione
> gira su una sessione implicita, quindi **fuori** dalla transazione dell'unità di lavoro in corso.

### Repository mapped

Quando il documento deve divergere dal dominio — denormalizzazione, campi tecnici, nomi diversi,
versionamento dello schema — si usa `MongoDBMappedRepository<TDocument, TEntity, TKey>`.
`[Collection]` sta sul **documento**, mai sull'entità di dominio.

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

La classe base espone `Collection` e il suo alias `Documents`, entrambi di tipo
`IMongoCollection<TDocument>`.

Il mapping può essere delegato a Mapster con un `TypeAdapterConfig` bidirezionale, come descritto
in [DAL – Mapped repository](dataaccesslayer.md#mapped-repository):

```csharp
config.NewConfig<Product, ProductDocument>().TwoWays();
```

> L'estensione `PreserveTracking` di `CodeArchitects.Platform.Data.Mapster` **non ha effetto** su
> questo provider: MongoDB non ha change tracking.

## Seeding

Il seeding usa la stessa `DataSeed` degli altri provider.

```csharp
[SeedOrder(1)]
public class CategorySeed : DataSeed
{
  public override void Seed(ISeeder seeder) => seeder.Seed(
    new Category { ... },
    new Category { ... });
}
```

Registrazione e applicazione:

```csharp
builder.Services.AddData(cfg => cfg
    // ...
    .UseSeed<CategorySeed>()
    .AddSeedsFrom(typeof(CategorySeed).Assembly));   // oppure per scansione

WebApplication app = builder.Build();
app.Services.SeedMongo();                             // oppure await SeedMongoAsync()
```

- L'**ordine** è deterministico: `[SeedOrder]` (assente ⇒ `0`), a parità di ordine il nome del tipo.
- Il seeding è **idempotente per collection**: un seed è applicato solo a una collection vuota.
- Tutti i seed sono committati **in un'unica transazione**: un seeding interrotto non lascia il
  database popolato a metà. Richiede quindi un replica set.

## Testing

Gli integration test del provider usano [Testcontainers](https://dotnet.testcontainers.org/) con un
replica set a nodo singolo, indispensabile per le transazioni:

```csharp
MongoDbContainer container = new MongoDbBuilder()
  .WithImage("mongo:7.0")
  .WithReplicaSet()
  .Build();
```

## Limiti noti

Non ancora supportati, in ordine di impatto:

- **`Include`** — sia sulle navigazioni incorporate (dove sarebbe un no-op) sia sui riferimenti
  inter-aggregato. Solleva `NotSupportedException`.
- **Concorrenza ottimistica** — nessun token di versione.
- **Multitenancy e soft delete** — disponibili sul provider EF Core, non qui.
- **Change tracking** — non ha corrispettivo nel modello ad aggregati.
- **Chiavi composite**, **gestione degli indici**, **database multipli** con contesto tipizzato.
- **Interceptor** sulle operazioni.

## Pacchetti

- [`CodeArchitects.Platform.Data.MongoDB`](https://www.nuget.org/packages/CodeArchitects.Platform.Data.MongoDB)
- [`CodeArchitects.Platform.Data.MongoDB.DependencyInjection`](https://www.nuget.org/packages/CodeArchitects.Platform.Data.MongoDB.DependencyInjection)

Richiedono **MongoDB Server 4.4 o superiore** (requisito del driver 3.x).
