# DAL con Entity Framework Core 7

## Configurazione

Per utilizzare le funzionalità della libreria CodeArchitects.Platform.Data.EntityFrameworkCore all'interno di un'applicazione ASP.NET Core, è necessario configurare il container di Dependency Injection attraverso gli appositi metodi.

In particolare, dopo aver abilitato l'uso di EFCore attraverso

```cs
services.AddDbContext<MyDbContext>(...);
```

è necessario configurare l'intero DAL tramite il seguente metodo:

```cs
services.AddData<MyDbContext>();
```

## Seeding

Per effettuare il seeding del database, è sufficiente utilizzare l'overload di `AddData` che consente di specificare ulteriori opzioni e successivamente utilizzare il metodo `UseSeed`:

```cs
services.AddData<MyDbContext>(options => options.UseSeed<MyDataSeed>());
```

per poi effettuare il seeding tramite il metodo di estensione `Seed`, tipicamente dopo la chiamata al metodo `Migrate` (o `EnsureCreated` se non si utilizzano le migrazioni).

```cs
using (IServiceScope scope = app.ApplicationServices.CreateScope())
{
  MyDbContext context = scope.Services.GetRequiredService<MyDbContext>();
  context.Migrate();
  context.Seed();
}
```

## CAEP extensions

L'estensione CAEP per EFCore permette di abilitare una serie di funzionalità, tra cui:

- Multitenancy
- Soft delete

Per utilizzare questa estensione, utilizzare il metodo di estensione `UseCaep` del `DbContextOptionsBuilder` all'interno della chiamata ad `AddDbContext` (o all'interno del metodo `OnConfiguring` di `DbContext`).

```cs
services.AddDbContext<MyDbContext>(options => options
  .UseSqlServer(...)
  .UseCaep()); // Da inserire dopo la chiamata all'estensione del provider (UseSqlServer in questo caso)
```

## Multitenancy

Il multi-tenancy è un concetto utilizzato per descrivere la capacità di un'applicazione web di fornire servizi a più clienti (noti come "tenant") all'interno di un'unica istanza di software. Ciò significa che un'unica versione dell'applicazione è in grado di gestire dati e configurazioni per più clienti, utilizzando meccanismi di isolamento per garantire che i dati e le configurazioni dei singoli tenant non si interfaccino tra loro.

Il multi-tenancy può essere implementato in diversi modi, a seconda delle esigenze dell'applicazione. Ad esempio, può essere realizzato utilizzando database separati per ogni tenant (separate database), oppure utilizzando schemi separati all'interno dello stesso database (shared database, separate schema) per isolare i dati dei tenant.

Quando non è possibile adottare nessuna delle due soluzioni, è necessario utilizzare meccanismi di isolamento a livello di codice per garantire che le azioni di un tenant non interferiscano con gli altri.

Per abilitare la separazione a livello di codice, si può usare l'estensione `UseMultitenancy` all'interno di `UseCaep`:

```cs
services.AddDbContext<MyContext>(options => options
  .UseSqlServer("...")
  .UseCaep(caep => caep.UseMultitenancy()));
```
  
Per indicare che un'entità è multi-tenant è sufficiente utilizzare il metodo di estensione `IsMultiTenant` all'interno del metodo `OnModelCreating`. Supponendo di avere la seguente entità:

```cs
public class MyEntity
{
  public Guid Id { get; set; }
  public Guid TenantId { get; set; }
  ...
}
```

Si avrebbe:

```cs
modelBuilder.Entity<MyEntity>(entity =>
{
  entity.IsMultiTenant(x => x.TenantId);
});
```

Una volta fatto ciò, le query effettuate su questa entità saranno automaticamente filtrate secondo il tenant dell'utente corrente.

> Nota: Di default, è necessario utilizzare l'interfaccia IIdentityProfile per recuperare il tenant id dai claims dell'utente corrente.

## Soft delete

Il soft delete è un meccanismo che consente di eliminare logicamente un record dal database senza eliminarlo fisicamente. Invece di eliminare fisicamente il record dal database, viene impostato un indicatore che segnala che il record è stato eliminato. Questo meccanismo consente di mantenere una traccia dei record eliminati e di poterli eventualmente ripristinare in futuro. Il soft delete può essere implementato utilizzando una colonna di stato (ad esempio, una colonna `IsDeleted` con valori `true` e `false`).

Per abilitare il meccanismo di soft delete, utilizzare il metodo `UseSoftDelete` all'interno di `UseCaep`:

```cs
services.AddDbContext<MyContext>(options => options
  .UseSqlServer("...")
  .UseCaep(caep => caep.UseSoftDelete()));
```

Nel metodo `OnModelCreating`, utilizzare il metodo `IsSoftDelete` per marcare la proprietà da utilizzare come colonna di stato:

```cs
modelBuilder.Entity<MyEntity>(entity =>
{
  entity.IsSoftDelete(x => x.IsDeleted);
});
```
