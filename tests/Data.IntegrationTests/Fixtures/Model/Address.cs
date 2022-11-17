using Bogus;

namespace CodeArchitects.Platform.Data.Fixtures.Model;

public class Address
{
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public User? User { get; set; } // OTO aggregation, on dependent

  private static readonly Faker<Address> s_faker = new Faker<Address>()
    .RuleFor(address => address.Id, Guid.NewGuid)
    .RuleFor(address => address.Name, faker => faker.Address.FullAddress());

  public static Address One() => s_faker.Generate();
}
