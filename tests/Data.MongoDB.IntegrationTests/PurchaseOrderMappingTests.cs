using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using CodeArchitects.Platform.Data.Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// The demo mapping on its own, without a database: it must survive the strict validation and
/// round-trip every part of the aggregate.
/// </summary>
public class PurchaseOrderMappingTests
{
  private static IDataMapper CreateMapper() => new ServiceCollection()
    .AddDataMapster(typeof(PurchaseOrderMapping).Assembly)
    .BuildServiceProvider()
    .CreateScope().ServiceProvider.GetRequiredService<IDataMapper>();

  [Fact]
  public void Mapping_ShouldRoundTripTheAggregate()
  {
    // Arrange
    IDataMapper mapper = CreateMapper();
    PurchaseOrder order = PurchaseOrder.One();

    // Act
    PurchaseOrderDocument document = mapper.Map<PurchaseOrder, PurchaseOrderDocument>(order);
    PurchaseOrder mapped = mapper.Map<PurchaseOrderDocument, PurchaseOrder>(document);

    // Assert
    mapped.Should().BeEquivalentTo(order);
    mapped.Lines.Should().NotBeSameAs(order.Lines);
  }

  [Fact]
  public void Mapping_ShouldProduceTheDocumentShape()
  {
    // Arrange
    IDataMapper mapper = CreateMapper();
    PurchaseOrder order = PurchaseOrder.One();

    // Act
    PurchaseOrderDocument document = mapper.Map<PurchaseOrder, PurchaseOrderDocument>(order);

    // Assert
    document.Id.Should().Be(order.Id);
    document.Code.Should().Be(order.Number.Value);
    document.TotalAmount.Should().Be(order.Total.Amount);
    document.TotalCurrency.Should().Be(order.Total.Currency);
    document.Shipping.Line1.Should().Be(order.ShippingAddress.Street);
    document.Shipping.CountryCode.Should().Be(order.ShippingAddress.Country);
    document.Lines.Select(line => line.Qty).Should().Equal(order.Lines.Select(line => line.Quantity));
    document.CustomerId.Should().Be(order.CustomerId);
    document.SchemaVersion.Should().Be(PurchaseOrderMapping.CurrentSchemaVersion);
  }

  [Fact]
  public void Mapping_ShouldReverseTheRenamedMember_WithTwoWays()
  {
    // Arrange
    IDataMapper mapper = CreateMapper();
    Ticket ticket = new() { Id = "65f0c0ffee00000000000001", Title = "Title" };

    // Act
    TicketDocument document = mapper.Map<Ticket, TicketDocument>(ticket);

    // Assert
    document.Subject.Should().Be(ticket.Title);
    mapper.Map<TicketDocument, Ticket>(document).Should().BeEquivalentTo(ticket);
  }
}
