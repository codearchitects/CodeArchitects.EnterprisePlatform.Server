using CodeArchitects.Platform.Data.MongoDB.Mapster;
using Mapster;
using CodeArchitects.Platform.Data.Mapster;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB.Fixtures;

// ---------------------------------------------------------------------------------------------
// The domain model: value objects, an embedded collection and a reference to another aggregate.
// Nothing here knows about MongoDB.
// ---------------------------------------------------------------------------------------------

/// <summary>A value object with a single value, persisted as a plain string.</summary>
public sealed record PurchaseOrderNumber(string Value);

/// <summary>A value object with two values, flattened into the document.</summary>
public sealed record Money(decimal Amount, string Currency);

public sealed record Address(string Street, string City, string PostalCode, string Country);

/// <summary>Part of the aggregate: embedded in the document, it has no key of its own.</summary>
public sealed record OrderLine(Guid ProductId, int Quantity, Money UnitPrice);

/// <summary>The aggregate root.</summary>
public class PurchaseOrder
{
  public Guid Id { get; set; }
  public PurchaseOrderNumber Number { get; set; } = default!;
  public Money Total { get; set; } = default!;
  public Address ShippingAddress { get; set; } = default!;
  public List<OrderLine> Lines { get; set; } = [];

  /// <summary>A reference to another aggregate: only its key, never the aggregate itself.</summary>
  public Guid CustomerId { get; set; }

  public static PurchaseOrder One(Guid? customerId = null)
  {
    List<OrderLine> lines =
    [
      new(Guid.NewGuid(), Random.Shared.Next(1, 5), new Money(Random.Shared.Next(10, 100), "EUR")),
      new(Guid.NewGuid(), Random.Shared.Next(1, 5), new Money(Random.Shared.Next(10, 100), "EUR"))
    ];

    return new PurchaseOrder
    {
      Id = Guid.NewGuid(),
      Number = new PurchaseOrderNumber($"PO-{Random.Shared.Next(10000, 99999)}"),
      Total = new Money(lines.Sum(line => line.Quantity * line.UnitPrice.Amount), "EUR"),
      ShippingAddress = new Address("Via Roma 1", "Milano", "20100", "IT"),
      Lines = lines,
      CustomerId = customerId ?? Guid.NewGuid()
    };
  }
}

// ---------------------------------------------------------------------------------------------
// The persistence model: renamed and flattened fields, embedded sub-documents, a technical field.
// Only this one is registered in the MongoDB model.
// ---------------------------------------------------------------------------------------------

[CodeArchitects.Platform.Data.MongoDB.Collection("purchase_orders")]
public class PurchaseOrderDocument
{
  public Guid Id { get; set; }
  public string Code { get; set; } = string.Empty;
  public decimal TotalAmount { get; set; }
  public string TotalCurrency { get; set; } = string.Empty;
  public AddressDocument Shipping { get; set; } = default!;
  public List<OrderLineDocument> Lines { get; set; } = [];
  public Guid CustomerId { get; set; }

  /// <summary>Exists only in the document: written on every save, never read back into the domain.</summary>
  public int SchemaVersion { get; set; }
}

public class AddressDocument
{
  public string Line1 { get; set; } = string.Empty;
  public string City { get; set; } = string.Empty;
  public string Zip { get; set; } = string.Empty;
  public string CountryCode { get; set; } = string.Empty;
}

public class OrderLineDocument
{
  public Guid ProductId { get; set; }
  public int Qty { get; set; }
  public decimal Price { get; set; }
  public string Currency { get; set; } = string.Empty;
}

/// <summary>
/// Every rule of the demo. The configuration is strict, so each pair of types, nested ones included,
/// needs its own rule, and each destination member needs a source.
/// </summary>
public sealed class PurchaseOrderMapping : IRegister
{
  public const int CurrentSchemaVersion = 2;

  public void Register(TypeAdapterConfig config)
  {
    // Single-value value object <-> scalar.
    config.NewConfig<PurchaseOrderNumber, string>().MapWith(number => number.Value);
    config.NewConfig<string, PurchaseOrderNumber>().MapWith(value => new PurchaseOrderNumber(value));

    // Immutable value object: built by the rules below, then shared as it is instead of copied.
    config.NewConfig<Money, Money>().MapWith(money => money);

    // Aggregate root: renamed, flattened and technical fields. Lines, Id and CustomerId map by name.
    config.NewConfig<PurchaseOrder, PurchaseOrderDocument>()
      .Map(document => document.Code, order => order.Number)
      .Map(document => document.TotalAmount, order => order.Total.Amount)
      .Map(document => document.TotalCurrency, order => order.Total.Currency)
      .Map(document => document.Shipping, order => order.ShippingAddress)
      .Map(document => document.SchemaVersion, _ => CurrentSchemaVersion);

    config.NewConfig<PurchaseOrderDocument, PurchaseOrder>()
      .Map(order => order.Number, document => document.Code)
      .Map(order => order.Total, document => new Money(document.TotalAmount, document.TotalCurrency))
      .Map(order => order.ShippingAddress, document => document.Shipping);

    // Embedded sub-document.
    config.NewConfig<Address, AddressDocument>()
      .Map(document => document.Line1, address => address.Street)
      .Map(document => document.Zip, address => address.PostalCode)
      .Map(document => document.CountryCode, address => address.Country);

    config.NewConfig<AddressDocument, Address>()
      .MapWith(document => new Address(document.Line1, document.City, document.Zip, document.CountryCode));

    // Embedded array, with a value object flattened in each element.
    config.NewConfig<OrderLine, OrderLineDocument>()
      .Map(document => document.Qty, line => line.Quantity)
      .Map(document => document.Price, line => line.UnitPrice.Amount)
      .Map(document => document.Currency, line => line.UnitPrice.Currency);

    config.NewConfig<OrderLineDocument, OrderLine>()
      .MapWith(document => new OrderLine(document.ProductId, document.Qty, new Money(document.Price, document.Currency)));

    // Same shape but one renamed member: a single rule, reversed by TwoWays. Only the members
    // configured after TwoWays() are reversed.
    config.NewConfig<Ticket, TicketDocument>()
      .TwoWays()
      .Map(document => document.Subject, ticket => ticket.Title);
  }
}

/// <summary>A specialized repository: its query is written on the document and returns domain entities.</summary>
public class PurchaseOrderRepository(IDataContext context, IDataMapper mapper)
  : MongoDBMapsterRepository<PurchaseOrderDocument, PurchaseOrder, Guid>(context, mapper)
{
  public Task<List<PurchaseOrder>> FindByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default) =>
    ToEntitiesAsync(
      Documents.Find(Session, document => document.CustomerId == customerId).SortBy(document => document.Code),
      cancellationToken);
}

// ---------------------------------------------------------------------------------------------
// A key generated by the driver: the document key is an ObjectId stored from a string, and the
// domain entity must receive it.
// ---------------------------------------------------------------------------------------------

public class Ticket
{
  public string? Id { get; set; }
  public string Title { get; set; } = string.Empty;
}

[CodeArchitects.Platform.Data.MongoDB.Collection("tickets")]
public class TicketDocument
{
  [BsonRepresentation(BsonType.ObjectId)]
  public string? Id { get; set; }
  public string Subject { get; set; } = string.Empty;
}
