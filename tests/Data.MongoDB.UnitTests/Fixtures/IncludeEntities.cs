using MongoDB.Bson.Serialization.Attributes;

namespace CodeArchitects.Platform.Data.MongoDB.Fixtures;

// Internal on purpose: AddEntitiesFrom must not discover these types, they are registered
// explicitly by the include tests.

/// <summary>
/// An aggregate root with every shape of navigation the include validation distinguishes.
/// </summary>
[CodeArchitects.Platform.Data.MongoDB.Collection("includeOrders")]
internal class IncludeOrder
{
  public Guid Id { get; set; }

  public string? Code { get; set; }

  /// <summary>Embedded, single.</summary>
  public Shipping? Shipping { get; set; }

  /// <summary>Embedded, collection.</summary>
  public List<OrderLine> Lines { get; set; } = [];

  /// <summary>An aggregate root of another collection.</summary>
  public IncludeBuyer? Buyer { get; set; }

  /// <summary>A collection of aggregate roots of another collection.</summary>
  public List<IncludeBuyer> Watchers { get; set; } = [];

  /// <summary>Not persisted: an include would leave it empty.</summary>
  [BsonIgnore]
  public Shipping? Cached { get; set; }
}

internal class Shipping
{
  public Address? Address { get; set; }

  public IncludeBuyer? Courier { get; set; }
}

internal class Address
{
  public string? City { get; set; }
}

internal class OrderLine
{
  public Discount? Discount { get; set; }
}

internal class Discount
{
  public decimal Rate { get; set; }
}

[CodeArchitects.Platform.Data.MongoDB.Collection("includeBuyers")]
internal class IncludeBuyer
{
  public Guid Id { get; set; }
}
