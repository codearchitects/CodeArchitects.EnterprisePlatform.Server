using Bogus;

namespace CodeArchitects.Platform.Data.Fixtures.Model;

public class Customer
{
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public List<Cart>? Carts { get; set; } // OTM inter-aggregate, on principal
  public List<CustomerClaim>? Claims { get; set; } // OTM intra-aggregate, on principal
  public Address? Address { get; set; } // OTO intra-aggregate, on principal

  private static readonly Faker<Customer> s_faker = new Faker<Customer>()
    .RuleFor(customer => customer.Id, Guid.NewGuid)
    .RuleFor(customer => customer.Name, faker => faker.Name.FullName());

  public static Customer One() => s_faker.Generate();
}
