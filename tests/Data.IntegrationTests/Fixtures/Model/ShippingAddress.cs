using Bogus;

namespace CodeArchitects.Platform.Data.Fixtures.Model;

public class ShippingAddress
{
  public Guid Id { get; set; }
  public string? Name { get; set; }

  private static readonly Faker<ShippingAddress> s_faker = new Faker<ShippingAddress>()
    .RuleFor(address => address.Id, Guid.NewGuid)
    .RuleFor(address => address.Name, faker => faker.Address.FullAddress());

  public static ShippingAddress One() => s_faker.Generate();
}
