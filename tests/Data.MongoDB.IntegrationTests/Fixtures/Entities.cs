using Bogus;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB.Fixtures;

/// <summary>
/// An aggregate root whose children are embedded in the same document: no second collection,
/// no Include, and the whole aggregate is written atomically by construction.
/// </summary>
[CodeArchitects.Platform.Data.MongoDB.Collection("carts")]
public class Cart
{
  public Guid Id { get; set; }
  public string? Owner { get; set; }
  public List<CartItem> Items { get; set; } = [];

  private static readonly Faker<Cart> s_faker = new Faker<Cart>()
    .RuleFor(cart => cart.Id, Guid.NewGuid)
    .RuleFor(cart => cart.Owner, faker => faker.Name.FullName())
    .RuleFor(cart => cart.Items, faker => [.. Enumerable.Range(0, 2).Select(_ => CartItem.One())]);

  public static Cart One() => s_faker.Generate();
}

/// <summary>Embedded, not an entity: it has no collection and needs no key.</summary>
public class CartItem
{
  public string? Sku { get; set; }
  public int Quantity { get; set; }

  private static readonly Faker<CartItem> s_faker = new Faker<CartItem>()
    .RuleFor(item => item.Sku, faker => faker.Commerce.Ean13())
    .RuleFor(item => item.Quantity, faker => faker.Random.Int(1, 5));

  public static CartItem One() => s_faker.Generate();
}

/// <summary>
/// Holds a navigation typed as another aggregate root: including it would ask the provider to
/// resolve a reference between collections, which it does not do.
/// </summary>
[CodeArchitects.Platform.Data.MongoDB.Collection("invoices")]
public class Invoice
{
  public Guid Id { get; set; }
  public Customer? Customer { get; set; }
}

/// <summary>The persisted shape: only this one is registered in the model.</summary>
[CodeArchitects.Platform.Data.MongoDB.Collection("orders")]
public class OrderDocument
{
  public Guid Id { get; set; }
  public string? Code { get; set; }
  public decimal Total { get; set; }
}

/// <summary>The domain shape, deliberately different from the document.</summary>
public class Order
{
  public Guid Id { get; set; }
  public string Reference { get; set; } = string.Empty;
  public decimal Amount { get; set; }

  public static Order One() => new()
  {
    Id = Guid.NewGuid(),
    Reference = $"ORD-{Random.Shared.Next(1000, 9999)}",
    Amount = Random.Shared.Next(10, 500)
  };
}

/// <summary>Maps by hand, to keep the test independent of any mapping library.</summary>
public class OrderRepository(IDataContext context) : MongoDBMappedRepository<OrderDocument, Order, Guid>(context)
{
  /// <summary>Surfaces the protected members a derived repository would use for custom queries.</summary>
  public IMongoCollection<OrderDocument> ExposedDocuments => Documents;

  public IClientSessionHandle ExposedSession => Session;

  protected override Order TableToEntity(OrderDocument document) => new()
  {
    Id = document.Id,
    Reference = document.Code ?? string.Empty,
    Amount = document.Total
  };

  protected override OrderDocument EntityToTable(Order entity) => new()
  {
    Id = entity.Id,
    Code = entity.Reference,
    Total = entity.Amount
  };
}

/// <summary>Used by the seeding tests.</summary>
[SeedOrder(1)]
public sealed class CustomerSeed : DataSeed
{
  public static readonly Customer[] Customers = [Customer.One(), Customer.One()];

  public override void Seed(ISeeder seeder) => seeder.Seed(Customers);
}
