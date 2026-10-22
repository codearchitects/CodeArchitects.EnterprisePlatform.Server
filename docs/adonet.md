# ADO.NET

To use Dapper as the ORM, which in turn uses the ADO.NET implementation of the repository, set the `data.orm` field of the yml accordingly.

```yml
data:
  orm: Dapper
```

## Entities

The ADO.NET DAL uses the `ModelConfiguration` base class to configure entities and relationships. To configure the relational model, extend this class and declare the entities and the relationships between them.

Entities are declared with the `Entity` method, which takes the entity as its type parameter and, as its argument, a lambda that configures the entity's properties.

For example, suppose we have an entity modelled by the following yml:

```yml
entities:
  - name: Category
    description: A category
    type: entity
    useRepository: true
    fields:
      - name: name
        type: string
        description: Category name
      - name: description
        type: string
        description: Category description
```

Besides generating the entity itself, thanks to the `useRepository` flag this generates the following configuration:

```cs
public class MyModelConfiguration : ModelConfiguration
{
  protected override void Configure()
  {
    Entity<Category>(builder =>
    {
      // configuration
    });
  }
}
```

## Associations

Associations are defined with the `Aggregate` and `Associate` methods, which take the two entities to relate as type parameters and, as their argument, a lambda that configures the relationship through a fluent API.

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
public class MyModelConfiguration : ModelConfiguration
{
  protected override void Configure()
  {
    Aggregate<Customer, Address>(builder => builder
      .OneToOne()
      .Navigation(x => x.ShippingAddress)
      .InverseNavigation(x => x.Customer));
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
public class MyModelConfiguration : ModelConfiguration
{
  protected override void Configure()
  {
    Associate<Customer, Address>(builder => builder
      .OneToOne()
      .Navigation(x => x.ShippingAddress)
      .InverseNavigation(x => x.Customer));
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
public class MyModelConfiguration : ModelConfiguration
{
  protected override void Configure()
  {
    Aggregate<Cart, Product>(builder => builder
      .OneToMany()
      .Navigation(x => x.Products)
      .InverseNavigation(x => x.Cart));
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
public class MyModelConfiguration : ModelConfiguration
{
  protected override void Configure()
  {
    Associate<Cart, Product>(builder => builder
      .OneToMany()
      .Navigation(x => x.Products)
      .InverseNavigation(x => x.Cart));
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
public class MyModelConfiguration : ModelConfiguration
{
  protected override void Configure()
  {
    Associate<Student, Teacher>(builder => builder
      .ManyToMany()
      .Navigation(x => x.Teachers)
      .InverseNavigation(x => x.Students));
  }
}
```
