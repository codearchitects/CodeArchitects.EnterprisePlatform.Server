# DAL with Entity Framework Core 7

To use Entity Framework Core as the ORM, set the `data.orm` field of the yml accordingly.

```yml
data:
  orm: EntityFrameworkCore
```

## Configuration

To use the features of the CodeArchitects.Platform.Data.EntityFrameworkCore library in an ASP.NET Core application, configure the Dependency Injection container through the dedicated methods.

In particular, after enabling EFCore with

```cs
services.AddDbContext<MyDbContext>(...);
```

configure the whole DAL with the following method:

```cs
services.AddData<MyDbContext>();
```

## Seeding

To seed the database, use the `AddData` overload that accepts additional options and then call the `UseSeed` method:

```cs
services.AddData<MyDbContext>(options => options.UseSeed<MyDataSeed>());
```

then run the seeding with the `Seed` extension method, typically after the call to `Migrate` (or `EnsureCreated` if you do not use migrations).

```cs
using (IServiceScope scope = app.ApplicationServices.CreateScope())
{
  MyDbContext context = scope.Services.GetRequiredService<MyDbContext>();
  context.Migrate();
  context.Seed();
}
```

## CAEP extensions

The CAEP extension for EFCore enables a set of features, including:

- Multitenancy
- Soft delete

To use this extension, call the `UseCaep` extension method of `DbContextOptionsBuilder` inside the call to `AddDbContext` (or inside the `OnConfiguring` method of the `DbContext`).

```cs
services.AddDbContext<MyDbContext>(options => options
  .UseSqlServer(...)
  .UseCaep()); // Must come after the call to the provider extension (UseSqlServer in this case)
```

## Multitenancy

Multi-tenancy describes the ability of a web application to serve multiple customers (known as "tenants") from a single software instance. A single version of the application manages data and configuration for many customers, using isolation mechanisms to ensure that the data and configuration of each tenant do not interfere with one another.

Multi-tenancy can be implemented in several ways, depending on the needs of the application. For example, it can use a separate database for each tenant (separate database), or separate schemas within the same database (shared database, separate schema) to isolate tenant data.

When neither solution is possible, code-level isolation mechanisms are needed to ensure that the actions of one tenant do not interfere with the others.

To enable code-level separation, use the `UseMultitenancy` extension inside `UseCaep`:

```cs
services.AddDbContext<MyContext>(options => options
  .UseSqlServer("...")
  .UseCaep(caep => caep.UseMultitenancy()));
```
  
To mark an entity as multi-tenant, use the `IsMultiTenant` extension method inside the `OnModelCreating` method. Given the following entity:

```cs
public class MyEntity
{
  public Guid Id { get; set; }
  public Guid TenantId { get; set; }
  ...
}
```

you would write:

```cs
modelBuilder.Entity<MyEntity>(entity =>
{
  entity.IsMultiTenant(x => x.TenantId);
});
```

From then on, queries on this entity are automatically filtered by the tenant of the current user.

> Note: By default, the IIdentityProfile interface must be used to retrieve the tenant id from the claims of the current user.

## Soft delete

Soft delete is a mechanism that removes a record from the database logically, without deleting it physically. Instead of physically deleting the record, a flag is set to indicate that the record has been deleted. This keeps track of deleted records and makes it possible to restore them later. Soft delete can be implemented with a status column (for example, an `IsDeleted` column with values `true` and `false`).

To enable soft delete, use the `UseSoftDelete` method inside `UseCaep`:

```cs
services.AddDbContext<MyContext>(options => options
  .UseSqlServer("...")
  .UseCaep(caep => caep.UseSoftDelete()));
```

In the `OnModelCreating` method, use the `IsSoftDelete` method to mark the property to use as the status column:

```cs
modelBuilder.Entity<MyEntity>(entity =>
{
  entity.IsSoftDelete(x => x.IsDeleted);
});
```

## Associations

Association types are defined with the `Aggregate()` and `Associate()` extension methods, used together with the EFCore fluent configuration API.

### Configuring a one-to-one, intra-aggregate association

Suppose we have the entities `Customer` and `Address`, in a one-to-one relationship and part of the same aggregate, with `Customer` as the Aggregate Root.

```cs
public class Customer
{
  public Guid Id { get; set; }
  public Address? ShippingAddress { get; set; }
}

public class Address
{
  public Guid Id { get; set; }
  public Customer? Customer { get; set; }
}
```

The association can be modelled with the following yml:

```yml
associations:
  - type: aggregate
    multiplicity: one-to-one
    from:
      entity: Customer
      navigation: ShippingAddress
    to:
      entity: Address
      navigation: Customer
```

This generates the following configuration:

```cs
public class MyDbContext : DbContext
{
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Customer>(entity =>
    {
      entity
        .HasOne(x => x.ShippingAddress)
        .WithOne(x => x.Customer)
        .Aggregate()
        .HasForeignKey<Address>();
    });
  }
}
```

### Configuring a one-to-one, inter-aggregate association

Suppose we have the entities `Customer` and `Address`, in a one-to-one relationship and not part of the same aggregate.

```cs
public class Customer
{
  public Guid Id { get; set; }
  public Address? ShippingAddress { get; set; }
}

public class Address
{
  public Guid Id { get; set; }
  public Customer? Customer { get; set; }
}
```

The association can be modelled with the following yml:

```yml
associations:
  - type: associate
    multiplicity: one-to-one
    from:
      entity: Customer
      navigation: ShippingAddress
    to:
      entity: Address
      navigation: Customer
```

This generates the following configuration:

```cs
public class MyDbContext : DbContext
{
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Customer>(entity =>
    {
      entity
        .HasOne(x => x.ShippingAddress)
        .WithOne(x => x.Customer)
        .Associate()
        .HasForeignKey<Address>();
    });
  }
}
```

### Configuring a one-to-many, intra-aggregate association

Suppose we have the entities `Cart` and `Product`, in a one-to-many relationship and part of the same aggregate, with `Cart` as the Aggregate Root.

```cs
public class Cart
{
  public Guid Id { get; set; }
  public IEnumerable<Product>? Products { get; set; }
}

public class Product
{
  public Guid Id { get; set; }
  public Cart? Cart { get; set; }
}
```

The association can be modelled with the following yml:

```yml
associations:
  - type: aggregate
    multiplicity: one-to-many
    from:
      entity: Cart
      navigation: Products
    to:
      entity: Product
      navigation: Cart
```

This generates the following configuration:

```cs
public class MyDbContext : DbContext
{
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Product>(entity =>
    {
      entity
        .HasMany(x => x.Products)
        .WithOne(x => x.Cart)
        .Aggregate();
    });
  }
}
```

### Configuring a one-to-many, inter-aggregate association

Suppose we have the entities `Cart` and `Product`, in a one-to-many relationship and not part of the same aggregate.

```cs
public class Cart
{
  public Guid Id { get; set; }
  public IEnumerable<Product>? Products { get; set; }
}

public class Product
{
  public Guid Id { get; set; }
  public Cart? Cart { get; set; }
}
```

The association can be modelled with the following yml:

```yml
associations:
  - type: associate
    multiplicity: one-to-many
    from:
      entity: Cart
      navigation: Products
    to:
      entity: Product
      navigation: Cart
```

This generates the following configuration:

```cs
public class MyDbContext : DbContext
{
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Product>(entity =>
    {
      entity
        .HasMany(x => x.Products)
        .WithOne(x => x.Cart)
        .Associate();
    });
  }
}
```

### Configuring a many-to-many association

Suppose we have the entities `Student` and `Teacher`, in a many-to-many relationship.

```cs
public class Student
{
  public Guid Id { get; set; }
  public IEnumerable<Teacher>? Teachers { get; set; }
}

public class Teacher
{
  public Guid Id { get; set; }
  public IEnumerable<Student>? Students { get; set; }
}
```

The association can be modelled with the following yml:

```yml
associations:
  - type: associate
    multiplicity: many-to-many
    from:
      entity: Student
      navigation: Teachers
    to:
      entity: Teacher
      navigation: Students
```

This generates the following configuration:

```cs
public class MyDbContext : DbContext
{
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Student>(entity =>
    {
      entity
        .HasMany(x => x.Teachers)
        .WithMany(x => x.Students);
    });
  }
}
```
